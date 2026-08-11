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

SELECT 'applications(tournamentId,playerid)' AS relationship_name, COUNT(*) AS duplicate_group_count
FROM (
    SELECT 1
    FROM applications
    GROUP BY tournamentId, playerid
    HAVING COUNT(*) > 1
) duplicate_applications
UNION ALL
SELECT 'team_applications(teamId,tournamentId)', COUNT(*)
FROM (
    SELECT 1
    FROM team_applications
    GROUP BY teamId, tournamentId
    HAVING COUNT(*) > 1
) duplicate_team_applications
UNION ALL
SELECT 'team_join_request(team_id,user_id)', COUNT(*)
FROM (
    SELECT 1
    FROM team_join_request
    GROUP BY team_id, user_id
    HAVING COUNT(*) > 1
) duplicate_join_requests;

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
SELECT 'applications.tournamentId', COUNT(*)
FROM applications a LEFT JOIN tournament t ON t.id = a.tournamentId
WHERE t.id IS NULL
UNION ALL
SELECT 'applications.playerid', COUNT(*)
FROM applications a LEFT JOIN `user` u ON u.id = a.playerid
WHERE u.id IS NULL
UNION ALL
SELECT 'team_applications.teamId', COUNT(*)
FROM team_applications ta LEFT JOIN team t ON t.id = ta.teamId
WHERE t.id IS NULL
UNION ALL
SELECT 'team_applications.tournamentId', COUNT(*)
FROM team_applications ta LEFT JOIN tournament t ON t.id = ta.tournamentId
WHERE t.id IS NULL
UNION ALL
SELECT 'team_join_request.team_id', COUNT(*)
FROM team_join_request tjr LEFT JOIN team t ON t.id = tjr.team_id
WHERE t.id IS NULL
UNION ALL
SELECT 'team_join_request.user_id', COUNT(*)
FROM team_join_request tjr LEFT JOIN `user` u ON u.id = tjr.user_id
WHERE u.id IS NULL
UNION ALL
SELECT 'matches.tournamentId', COUNT(*)
FROM matches m LEFT JOIN tournament t ON t.id = m.tournamentId
WHERE t.id IS NULL
UNION ALL
SELECT 'matches.user_id1', COUNT(*)
FROM matches m LEFT JOIN `user` u ON u.id = m.user_id1
WHERE u.id IS NULL
UNION ALL
SELECT 'matches.user_id2', COUNT(*)
FROM matches m LEFT JOIN `user` u ON u.id = m.user_id2
WHERE u.id IS NULL
UNION ALL
SELECT 'team_matches.tournamentId', COUNT(*)
FROM team_matches tm LEFT JOIN tournament t ON t.id = tm.tournamentId
WHERE t.id IS NULL
UNION ALL
SELECT 'team_matches.team_id1', COUNT(*)
FROM team_matches tm LEFT JOIN team t ON t.id = tm.team_id1
WHERE t.id IS NULL
UNION ALL
SELECT 'team_matches.team_id2', COUNT(*)
FROM team_matches tm LEFT JOIN team t ON t.id = tm.team_id2
WHERE t.id IS NULL;

SELECT 'invalid_tournament_status' AS check_name, COUNT(*) AS problem_count
FROM tournament
WHERE status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'invalid_tournament_team_size', COUNT(*)
FROM tournament
WHERE team_size_required NOT BETWEEN 1 AND 5
UNION ALL
SELECT 'invalid_match_status', COUNT(*)
FROM matches
WHERE status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'invalid_team_match_status', COUNT(*)
FROM team_matches
WHERE status_int IS NULL OR status_int NOT BETWEEN 0 AND 2
UNION ALL
SELECT 'same_match_players', COUNT(*)
FROM matches
WHERE user_id1 = user_id2
UNION ALL
SELECT 'same_match_teams', COUNT(*)
FROM team_matches
WHERE team_id1 = team_id2
UNION ALL
SELECT 'invalid_match_scores', COUNT(*)
FROM matches
WHERE player1Score IS NULL OR player2Score IS NULL
   OR player1Score NOT BETWEEN 0 AND 99
   OR player2Score NOT BETWEEN 0 AND 99
UNION ALL
SELECT 'invalid_team_match_scores', COUNT(*)
FROM team_matches
WHERE team1_score IS NULL OR team2_score IS NULL
   OR team1_score NOT BETWEEN 0 AND 99
   OR team2_score NOT BETWEEN 0 AND 99
UNION ALL
SELECT 'nullable_notifications', COUNT(*)
FROM notification
WHERE is_read IS NULL OR created_at IS NULL;

SELECT CONSTRAINT_NAME, TABLE_NAME, REFERENCED_TABLE_NAME,
       UPDATE_RULE, DELETE_RULE
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA = DATABASE()
ORDER BY TABLE_NAME, CONSTRAINT_NAME;
