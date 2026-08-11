-- Read-only C5GO database audit. This script does not change data or schema.

SELECT DATABASE() AS database_name, VERSION() AS mysql_version;

SELECT TABLE_NAME, ENGINE, TABLE_COLLATION, TABLE_ROWS
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = DATABASE()
ORDER BY TABLE_NAME;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE,
       GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) AS columns_in_index
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
GROUP BY TABLE_NAME, INDEX_NAME, NON_UNIQUE
ORDER BY TABLE_NAME, INDEX_NAME;

SELECT LOWER(TRIM(username)) AS normalized_username, COUNT(*) AS duplicate_count
FROM `user`
GROUP BY LOWER(TRIM(username))
HAVING COUNT(*) > 1;

SELECT LOWER(TRIM(email)) AS normalized_email, COUNT(*) AS duplicate_count
FROM `user`
GROUP BY LOWER(TRIM(email))
HAVING COUNT(*) > 1;

SELECT steam_id, COUNT(*) AS duplicate_count
FROM `user`
WHERE steam_id IS NOT NULL AND steam_id NOT IN ('', '0')
GROUP BY steam_id
HAVING COUNT(*) > 1;

SELECT id, username, steam_id
FROM `user`
WHERE steam_id IS NOT NULL
  AND steam_id NOT IN ('', '0')
  AND (
      CHAR_LENGTH(steam_id) <> 17
      OR steam_id NOT REGEXP '^[0-9]{17}$'
      OR steam_id NOT LIKE '7656119%'
  )
ORDER BY id;

SELECT 'post.authorid' AS relationship_name, COUNT(*) AS orphan_count
FROM post p LEFT JOIN `user` u ON u.id = p.authorid
WHERE u.id IS NULL
UNION ALL
SELECT 'comment.authorid', COUNT(*)
FROM comment c LEFT JOIN `user` u ON u.id = c.authorid
WHERE u.id IS NULL
UNION ALL
SELECT 'comment.post_id', COUNT(*)
FROM comment c LEFT JOIN post p ON p.id = c.post_id
WHERE p.id IS NULL
UNION ALL
SELECT 'commentreply.comment_id', COUNT(*)
FROM commentreply cr LEFT JOIN comment c ON c.id = cr.comment_id
WHERE c.id IS NULL
UNION ALL
SELECT 'commentreply.user_id', COUNT(*)
FROM commentreply cr LEFT JOIN `user` u ON u.id = cr.user_id
WHERE u.id IS NULL
UNION ALL
SELECT 'notification.user_id', COUNT(*)
FROM notification n LEFT JOIN `user` u ON u.id = n.user_id
WHERE u.id IS NULL
UNION ALL
SELECT 'team.captain_id', COUNT(*)
FROM team t LEFT JOIN `user` u ON u.id = t.captain_id
WHERE u.id IS NULL
UNION ALL
SELECT 'team_player.team_id', COUNT(*)
FROM team_player tp LEFT JOIN team t ON t.id = tp.team_id
WHERE t.id IS NULL
UNION ALL
SELECT 'team_player.user_id', COUNT(*)
FROM team_player tp LEFT JOIN `user` u ON u.id = tp.user_id
WHERE u.id IS NULL
UNION ALL
SELECT 'matches.tournamentId', COUNT(*)
FROM matches m LEFT JOIN tournament t ON t.id = m.tournamentId
WHERE t.id IS NULL
UNION ALL
SELECT 'team_matches.tournamentId', COUNT(*)
FROM team_matches tm LEFT JOIN tournament t ON t.id = tm.tournamentId
WHERE t.id IS NULL;

SELECT CONSTRAINT_NAME, TABLE_NAME, REFERENCED_TABLE_NAME,
       UPDATE_RULE, DELETE_RULE
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA = DATABASE()
ORDER BY TABLE_NAME, CONSTRAINT_NAME;
