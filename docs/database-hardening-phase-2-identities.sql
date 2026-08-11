-- C5GO database hardening, phase 2.
-- Run this only after every duplicate username, email and SteamID has been reviewed manually.

CREATE TEMPORARY TABLE c5go_phase2_assertions (
    check_name VARCHAR(100) NOT NULL,
    problem_count BIGINT NOT NULL CHECK (problem_count = 0)
);

INSERT INTO c5go_phase2_assertions
SELECT 'blank usernames or emails', COUNT(*)
FROM `user`
WHERE TRIM(username) = '' OR TRIM(email) = ''
UNION ALL
SELECT 'duplicate normalized usernames', COUNT(*)
FROM (
    SELECT 1 FROM `user`
    GROUP BY LOWER(TRIM(username))
    HAVING COUNT(*) > 1
) duplicate_usernames
UNION ALL
SELECT 'duplicate normalized emails', COUNT(*)
FROM (
    SELECT 1 FROM `user`
    GROUP BY LOWER(TRIM(email))
    HAVING COUNT(*) > 1
) duplicate_emails
UNION ALL
SELECT 'duplicate SteamIDs', COUNT(*)
FROM (
    SELECT 1 FROM `user`
    WHERE steam_id IS NOT NULL AND TRIM(steam_id) NOT IN ('', '0')
    GROUP BY steam_id
    HAVING COUNT(*) > 1
) duplicate_steam_ids
UNION ALL
SELECT 'invalid SteamIDs', COUNT(*)
FROM `user`
WHERE steam_id IS NOT NULL
  AND TRIM(steam_id) NOT IN ('', '0')
  AND (
      CHAR_LENGTH(TRIM(steam_id)) <> 17
      OR TRIM(steam_id) NOT REGEXP '^[0-9]{17}$'
      OR TRIM(steam_id) NOT LIKE '7656119%'
  )
UNION ALL
SELECT 'phase 2 already applied', COUNT(*)
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND INDEX_NAME IN ('uq_user_username', 'uq_user_email', 'uq_user_steam_id');

DROP TEMPORARY TABLE c5go_phase2_assertions;

UPDATE `user`
SET username = TRIM(username),
    email = LOWER(TRIM(email)),
    steam_id = CASE
        WHEN steam_id IS NULL OR TRIM(steam_id) IN ('', '0') THEN NULL
        ELSE TRIM(steam_id)
    END;

ALTER TABLE `user`
    MODIFY steam_id VARCHAR(17) NULL,
    ADD CONSTRAINT uq_user_username UNIQUE (username),
    ADD CONSTRAINT uq_user_email UNIQUE (email),
    ADD CONSTRAINT uq_user_steam_id UNIQUE (steam_id),
    ADD CONSTRAINT chk_user_steam_id_shape CHECK (
        steam_id IS NULL
        OR (CHAR_LENGTH(steam_id) = 17 AND steam_id LIKE '7656119%')
    );
