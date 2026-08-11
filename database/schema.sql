-- C5GO schema for a new, empty MySQL 8.0.16+ database.
-- Select the target database before running this script.

SET NAMES utf8mb4;

CREATE TABLE `user` (
    id INT NOT NULL AUTO_INCREMENT,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    birthday DATE NULL,
    age INT NULL,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    password VARCHAR(255) NOT NULL,
    is_moderator TINYINT(1) NOT NULL DEFAULT 0,
    steam_id VARCHAR(17) NULL,
    email_confirmed TINYINT(1) NOT NULL DEFAULT 0,
    email_token VARCHAR(255) NULL,
    token_created_at DATETIME NULL,
    CONSTRAINT pk_user PRIMARY KEY (id),
    CONSTRAINT uq_user_username UNIQUE (username),
    CONSTRAINT uq_user_email UNIQUE (email),
    CONSTRAINT uq_user_steam_id UNIQUE (steam_id),
    CONSTRAINT uq_user_email_token UNIQUE (email_token),
    CONSTRAINT chk_user_steam_id_shape CHECK (
        steam_id IS NULL
        OR (CHAR_LENGTH(steam_id) = 17 AND steam_id LIKE '7656119%')
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE post (
    id INT NOT NULL AUTO_INCREMENT,
    authorid INT NOT NULL,
    content TEXT NOT NULL,
    posted_on DATETIME NOT NULL,
    title VARCHAR(255) NOT NULL,
    image_path VARCHAR(255) NULL,
    CONSTRAINT pk_post PRIMARY KEY (id),
    CONSTRAINT fk_post_author FOREIGN KEY (authorid)
        REFERENCES `user` (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE comment (
    id INT NOT NULL AUTO_INCREMENT,
    authorid INT NOT NULL,
    content VARCHAR(255) NOT NULL,
    posted_on DATETIME NOT NULL,
    post_id INT NOT NULL,
    CONSTRAINT pk_comment PRIMARY KEY (id),
    CONSTRAINT fk_comment_author FOREIGN KEY (authorid)
        REFERENCES `user` (id),
    CONSTRAINT fk_comment_post FOREIGN KEY (post_id)
        REFERENCES post (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE commentreply (
    id INT NOT NULL AUTO_INCREMENT,
    content VARCHAR(255) NOT NULL,
    posted_on DATETIME NOT NULL,
    comment_id INT NOT NULL,
    user_id INT NOT NULL,
    CONSTRAINT pk_commentreply PRIMARY KEY (id),
    CONSTRAINT fk_commentreply_comment FOREIGN KEY (comment_id)
        REFERENCES comment (id) ON DELETE CASCADE,
    CONSTRAINT fk_commentreply_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE tournament (
    id INT NOT NULL AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(300) NOT NULL,
    status_int INT NOT NULL DEFAULT 0,
    is_team TINYINT(1) NOT NULL DEFAULT 0,
    team_size_required INT NOT NULL DEFAULT 1,
    CONSTRAINT pk_tournament PRIMARY KEY (id),
    CONSTRAINT chk_tournament_status CHECK (status_int BETWEEN 0 AND 2),
    CONSTRAINT chk_tournament_team_size CHECK (team_size_required BETWEEN 1 AND 5)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE team (
    id INT NOT NULL AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    captain_id INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_team PRIMARY KEY (id),
    CONSTRAINT uq_team_name UNIQUE (name),
    CONSTRAINT fk_team_captain FOREIGN KEY (captain_id)
        REFERENCES `user` (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE team_player (
    team_id INT NOT NULL,
    user_id INT NOT NULL,
    role ENUM('Captain', 'Member') NULL DEFAULT 'Member',
    status ENUM('Pending', 'Approved') NULL DEFAULT 'Pending',
    CONSTRAINT pk_team_player PRIMARY KEY (team_id, user_id),
    CONSTRAINT fk_team_player_team FOREIGN KEY (team_id)
        REFERENCES team (id) ON DELETE CASCADE,
    CONSTRAINT fk_team_player_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE team_join_request (
    id INT NOT NULL AUTO_INCREMENT,
    team_id INT NOT NULL,
    user_id INT NOT NULL,
    requested_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_team_join_request PRIMARY KEY (id),
    CONSTRAINT uq_team_join_request_team_user UNIQUE (team_id, user_id),
    CONSTRAINT fk_team_join_request_team FOREIGN KEY (team_id)
        REFERENCES team (id) ON DELETE CASCADE,
    CONSTRAINT fk_team_join_request_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE applications (
    tournamentId INT NOT NULL,
    playerid INT NOT NULL,
    CONSTRAINT pk_applications PRIMARY KEY (tournamentId, playerid),
    CONSTRAINT fk_applications_tournament FOREIGN KEY (tournamentId)
        REFERENCES tournament (id) ON DELETE CASCADE,
    CONSTRAINT fk_applications_user FOREIGN KEY (playerid)
        REFERENCES `user` (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE team_applications (
    id INT NOT NULL AUTO_INCREMENT,
    teamId INT NOT NULL,
    tournamentId INT NOT NULL,
    CONSTRAINT pk_team_applications PRIMARY KEY (id),
    CONSTRAINT uq_team_applications_team_tournament UNIQUE (teamId, tournamentId),
    CONSTRAINT fk_team_applications_team FOREIGN KEY (teamId)
        REFERENCES team (id) ON DELETE CASCADE,
    CONSTRAINT fk_team_applications_tournament FOREIGN KEY (tournamentId)
        REFERENCES tournament (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE matches (
    id INT NOT NULL AUTO_INCREMENT,
    tournamentId INT NOT NULL,
    user_id1 INT NOT NULL,
    user_id2 INT NOT NULL,
    player1Score INT NOT NULL DEFAULT 0,
    player2Score INT NOT NULL DEFAULT 0,
    match_date DATETIME NOT NULL,
    status_int INT NOT NULL DEFAULT 0,
    CONSTRAINT pk_matches PRIMARY KEY (id),
    CONSTRAINT fk_matches_tournament FOREIGN KEY (tournamentId)
        REFERENCES tournament (id) ON DELETE CASCADE,
    CONSTRAINT fk_matches_user1 FOREIGN KEY (user_id1)
        REFERENCES `user` (id),
    CONSTRAINT fk_matches_user2 FOREIGN KEY (user_id2)
        REFERENCES `user` (id),
    CONSTRAINT chk_matches_status CHECK (status_int BETWEEN 0 AND 2),
    CONSTRAINT chk_matches_distinct_players CHECK (user_id1 <> user_id2),
    CONSTRAINT chk_matches_scores CHECK (
        player1Score BETWEEN 0 AND 99 AND player2Score BETWEEN 0 AND 99
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE team_matches (
    id INT NOT NULL AUTO_INCREMENT,
    tournamentId INT NOT NULL,
    team_id1 INT NOT NULL,
    team_id2 INT NOT NULL,
    team1_score INT NOT NULL DEFAULT 0,
    team2_score INT NOT NULL DEFAULT 0,
    match_date DATETIME NOT NULL,
    status_int INT NOT NULL DEFAULT 0,
    CONSTRAINT pk_team_matches PRIMARY KEY (id),
    CONSTRAINT fk_team_matches_tournament FOREIGN KEY (tournamentId)
        REFERENCES tournament (id) ON DELETE CASCADE,
    CONSTRAINT fk_team_matches_team1 FOREIGN KEY (team_id1)
        REFERENCES team (id),
    CONSTRAINT fk_team_matches_team2 FOREIGN KEY (team_id2)
        REFERENCES team (id),
    CONSTRAINT chk_team_matches_status CHECK (status_int BETWEEN 0 AND 2),
    CONSTRAINT chk_team_matches_distinct_teams CHECK (team_id1 <> team_id2),
    CONSTRAINT chk_team_matches_scores CHECK (
        team1_score BETWEEN 0 AND 99 AND team2_score BETWEEN 0 AND 99
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE notification (
    id INT NOT NULL AUTO_INCREMENT,
    user_id INT NOT NULL,
    message VARCHAR(255) NOT NULL,
    link VARCHAR(255) NULL,
    is_read TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_notification PRIMARY KEY (id),
    INDEX idx_notification_user_unread_created (user_id, is_read, created_at),
    CONSTRAINT fk_notification_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
