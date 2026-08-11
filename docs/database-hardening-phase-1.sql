-- C5GO database hardening, phase 1.
-- Back up the database and run database-security-audit.sql before this migration.
-- MySQL 8.0.16 or newer is required because this migration adds enforced CHECK constraints.

CREATE TEMPORARY TABLE c5go_phase1_assertions (
    check_name VARCHAR(100) NOT NULL,
    problem_count BIGINT NOT NULL CHECK (problem_count = 0)
);

INSERT INTO c5go_phase1_assertions
SELECT 'duplicate applications', COUNT(*)
FROM (
    SELECT 1 FROM applications
    GROUP BY tournamentId, playerid
    HAVING COUNT(*) > 1
) duplicate_applications
UNION ALL
SELECT 'duplicate team applications', COUNT(*)
FROM (
    SELECT 1 FROM team_applications
    GROUP BY teamId, tournamentId
    HAVING COUNT(*) > 1
) duplicate_team_applications
UNION ALL
SELECT 'duplicate team join requests', COUNT(*)
FROM (
    SELECT 1 FROM team_join_request
    GROUP BY team_id, user_id
    HAVING COUNT(*) > 1
) duplicate_join_requests
UNION ALL
SELECT 'duplicate email verification tokens', COUNT(*)
FROM (
    SELECT 1 FROM `user`
    WHERE email_token IS NOT NULL
    GROUP BY email_token
    HAVING COUNT(*) > 1
) duplicate_email_tokens
UNION ALL
SELECT 'orphan notifications', COUNT(*)
FROM notification n LEFT JOIN `user` u ON u.id = n.user_id
WHERE u.id IS NULL
UNION ALL
SELECT 'invalid tournament statuses', COUNT(*)
FROM tournament WHERE status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'invalid tournament team sizes', COUNT(*)
FROM tournament WHERE team_size_required NOT BETWEEN 1 AND 5
UNION ALL
SELECT 'invalid solo match statuses', COUNT(*)
FROM matches WHERE status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'same player on both match sides', COUNT(*)
FROM matches WHERE user_id1 = user_id2
UNION ALL
SELECT 'invalid solo match scores', COUNT(*)
FROM matches
WHERE player1Score IS NOT NULL AND player1Score NOT BETWEEN 0 AND 99
   OR player2Score IS NOT NULL AND player2Score NOT BETWEEN 0 AND 99
UNION ALL
SELECT 'invalid team match statuses', COUNT(*)
FROM team_matches
WHERE status_int IS NOT NULL AND status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'same team on both match sides', COUNT(*)
FROM team_matches WHERE team_id1 = team_id2
UNION ALL
SELECT 'invalid team match scores', COUNT(*)
FROM team_matches
WHERE team1_score IS NOT NULL AND team1_score NOT BETWEEN 0 AND 99
   OR team2_score IS NOT NULL AND team2_score NOT BETWEEN 0 AND 99
UNION ALL
SELECT 'phase 1 already applied', COUNT(*)
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND INDEX_NAME IN (
      'uq_team_applications_team_tournament',
      'uq_team_join_request_team_user',
      'uq_user_email_token',
      'idx_notification_user_unread_created'
  );

DROP TEMPORARY TABLE c5go_phase1_assertions;

ALTER TABLE tournament
    MODIFY description VARCHAR(300) NOT NULL;

UPDATE matches
SET player1Score = COALESCE(player1Score, 0),
    player2Score = COALESCE(player2Score, 0);

ALTER TABLE matches
    MODIFY player1Score INT NOT NULL DEFAULT 0,
    MODIFY player2Score INT NOT NULL DEFAULT 0,
    ADD CONSTRAINT chk_matches_status CHECK (status_int BETWEEN 0 AND 2),
    ADD CONSTRAINT chk_matches_distinct_players CHECK (user_id1 <> user_id2),
    ADD CONSTRAINT chk_matches_scores CHECK (
        player1Score BETWEEN 0 AND 99 AND player2Score BETWEEN 0 AND 99
    );

UPDATE team_matches
SET team1_score = COALESCE(team1_score, 0),
    team2_score = COALESCE(team2_score, 0),
    status_int = COALESCE(status_int, 0);

ALTER TABLE team_matches
    MODIFY team1_score INT NOT NULL DEFAULT 0,
    MODIFY team2_score INT NOT NULL DEFAULT 0,
    MODIFY status_int INT NOT NULL DEFAULT 0,
    ADD CONSTRAINT chk_team_matches_status CHECK (status_int BETWEEN 0 AND 2),
    ADD CONSTRAINT chk_team_matches_distinct_teams CHECK (team_id1 <> team_id2),
    ADD CONSTRAINT chk_team_matches_scores CHECK (
        team1_score BETWEEN 0 AND 99 AND team2_score BETWEEN 0 AND 99
    );

ALTER TABLE tournament
    ADD CONSTRAINT chk_tournament_status CHECK (status_int BETWEEN 0 AND 2),
    ADD CONSTRAINT chk_tournament_team_size CHECK (team_size_required BETWEEN 1 AND 5);

UPDATE notification
SET is_read = COALESCE(is_read, 0),
    created_at = COALESCE(created_at, UTC_TIMESTAMP());

ALTER TABLE notification
    MODIFY is_read TINYINT(1) NOT NULL DEFAULT 0,
    MODIFY created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ADD INDEX idx_notification_user_unread_created (user_id, is_read, created_at),
    ADD CONSTRAINT fk_notification_user
        FOREIGN KEY (user_id) REFERENCES `user` (id) ON DELETE CASCADE;

ALTER TABLE applications
    ADD PRIMARY KEY (tournamentId, playerid);

ALTER TABLE team_applications
    ADD CONSTRAINT uq_team_applications_team_tournament
        UNIQUE (teamId, tournamentId);

ALTER TABLE team_join_request
    ADD CONSTRAINT uq_team_join_request_team_user
        UNIQUE (team_id, user_id);

ALTER TABLE `user`
    ADD CONSTRAINT uq_user_email_token UNIQUE (email_token);
