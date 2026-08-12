-- C5GO Community foundation.
-- Back up the database before running this additive migration.
-- MySQL 8.0.16 or newer is required for enforced CHECK constraints.

CREATE TABLE IF NOT EXISTS platform_role (
    id TINYINT UNSIGNED NOT NULL,
    name VARCHAR(20) NOT NULL,
    hierarchy_level TINYINT UNSIGNED NOT NULL,
    CONSTRAINT pk_platform_role PRIMARY KEY (id),
    CONSTRAINT uq_platform_role_name UNIQUE (name),
    CONSTRAINT uq_platform_role_level UNIQUE (hierarchy_level),
    CONSTRAINT chk_platform_role_level CHECK (hierarchy_level BETWEEN 1 AND 4)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT IGNORE INTO platform_role (id, name, hierarchy_level) VALUES
    (1, 'Member', 1),
    (2, 'Moderator', 2),
    (3, 'Admin', 3),
    (4, 'Owner', 4);

CREATE TABLE IF NOT EXISTS user_role (
    user_id INT NOT NULL,
    role_id TINYINT UNSIGNED NOT NULL,
    assigned_by INT NULL,
    assigned_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reason VARCHAR(255) NULL,
    CONSTRAINT pk_user_role PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_role_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE,
    CONSTRAINT fk_user_role_role FOREIGN KEY (role_id)
        REFERENCES platform_role (id),
    CONSTRAINT fk_user_role_assigned_by FOREIGN KEY (assigned_by)
        REFERENCES `user` (id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT IGNORE INTO user_role (user_id, role_id, assigned_by, assigned_at, reason)
SELECT id, 1, NULL, UTC_TIMESTAMP(), 'Initial Member role'
FROM `user`;

INSERT IGNORE INTO user_role (user_id, role_id, assigned_by, assigned_at, reason)
SELECT id, 3, NULL, UTC_TIMESTAMP(), 'Migrated from legacy administrator flag'
FROM `user`
WHERE is_moderator = 1;

CREATE TABLE IF NOT EXISTS role_assignment_audit (
    id BIGINT NOT NULL AUTO_INCREMENT,
    user_id INT NULL,
    role_id TINYINT UNSIGNED NOT NULL,
    action_type TINYINT UNSIGNED NOT NULL,
    performed_by INT NULL,
    reason VARCHAR(255) NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_role_assignment_audit PRIMARY KEY (id),
    CONSTRAINT fk_role_audit_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT fk_role_audit_role FOREIGN KEY (role_id)
        REFERENCES platform_role (id),
    CONSTRAINT fk_role_audit_performed_by FOREIGN KEY (performed_by)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT chk_role_audit_action CHECK (action_type BETWEEN 0 AND 1),
    INDEX idx_role_audit_user_created (user_id, created_at),
    INDEX idx_role_audit_actor_created (performed_by, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT INTO role_assignment_audit
    (user_id, role_id, action_type, performed_by, reason, created_at)
SELECT user_role.user_id, 3, 0, NULL,
       'Migrated from legacy administrator flag', user_role.assigned_at
FROM user_role
WHERE user_role.role_id = 3
  AND NOT EXISTS (
      SELECT 1
      FROM role_assignment_audit audit
      WHERE audit.user_id = user_role.user_id
        AND audit.role_id = 3
        AND audit.action_type = 0
  );

CREATE TABLE IF NOT EXISTS community_category (
    id INT NOT NULL AUTO_INCREMENT,
    slug VARCHAR(60) NOT NULL,
    name VARCHAR(80) NOT NULL,
    description VARCHAR(255) NOT NULL,
    display_order INT NOT NULL DEFAULT 0,
    is_active TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT pk_community_category PRIMARY KEY (id),
    CONSTRAINT uq_community_category_slug UNIQUE (slug),
    CONSTRAINT uq_community_category_name UNIQUE (name),
    CONSTRAINT chk_community_category_order CHECK (display_order >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT IGNORE INTO community_category
    (slug, name, description, display_order, is_active)
VALUES
    ('general', 'General', 'General Counter-Strike community discussions.', 1, 1),
    ('matches', 'Matches', 'Professional and community match discussions.', 2, 1),
    ('teams-players', 'Teams and players', 'Teams, players, transfers and performance.', 3, 1),
    ('tournaments', 'Tournaments', 'Tournament news, questions and predictions.', 4, 1);

CREATE TABLE IF NOT EXISTS discussion (
    id INT NOT NULL AUTO_INCREMENT,
    author_id INT NOT NULL,
    category_id INT NOT NULL,
    title VARCHAR(160) NOT NULL,
    content TEXT NULL,
    image_path VARCHAR(255) NULL,
    youtube_video_id VARCHAR(20) NULL,
    is_spoiler TINYINT(1) NOT NULL DEFAULT 0,
    is_locked TINYINT(1) NOT NULL DEFAULT 0,
    is_pinned TINYINT(1) NOT NULL DEFAULT 0,
    status_int TINYINT UNSIGNED NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL,
    removed_at DATETIME NULL,
    removed_by INT NULL,
    removal_reason VARCHAR(255) NULL,
    CONSTRAINT pk_discussion PRIMARY KEY (id),
    CONSTRAINT fk_discussion_author FOREIGN KEY (author_id)
        REFERENCES `user` (id),
    CONSTRAINT fk_discussion_category FOREIGN KEY (category_id)
        REFERENCES community_category (id),
    CONSTRAINT fk_discussion_removed_by FOREIGN KEY (removed_by)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT chk_discussion_title CHECK (CHAR_LENGTH(TRIM(title)) BETWEEN 5 AND 160),
    CONSTRAINT chk_discussion_status CHECK (status_int BETWEEN 0 AND 1),
    CONSTRAINT chk_discussion_content CHECK (
        (content IS NOT NULL AND CHAR_LENGTH(TRIM(content)) > 0)
        OR image_path IS NOT NULL
        OR youtube_video_id IS NOT NULL
    ),
    INDEX idx_discussion_feed (status_int, is_pinned, created_at),
    INDEX idx_discussion_category_feed (category_id, status_int, created_at),
    INDEX idx_discussion_author_created (author_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS discussion_vote (
    discussion_id INT NOT NULL,
    user_id INT NOT NULL,
    vote_value TINYINT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL,
    CONSTRAINT pk_discussion_vote PRIMARY KEY (discussion_id, user_id),
    CONSTRAINT fk_discussion_vote_discussion FOREIGN KEY (discussion_id)
        REFERENCES discussion (id) ON DELETE CASCADE,
    CONSTRAINT fk_discussion_vote_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE,
    CONSTRAINT chk_discussion_vote_value CHECK (vote_value IN (-1, 1)),
    INDEX idx_discussion_vote_user_created (user_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS discussion_comment (
    id INT NOT NULL AUTO_INCREMENT,
    discussion_id INT NOT NULL,
    author_id INT NOT NULL,
    parent_comment_id INT NULL,
    content VARCHAR(2000) NOT NULL,
    status_int TINYINT UNSIGNED NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL,
    removed_at DATETIME NULL,
    removed_by INT NULL,
    removal_reason VARCHAR(255) NULL,
    CONSTRAINT pk_discussion_comment PRIMARY KEY (id),
    CONSTRAINT fk_discussion_comment_discussion FOREIGN KEY (discussion_id)
        REFERENCES discussion (id) ON DELETE CASCADE,
    CONSTRAINT fk_discussion_comment_author FOREIGN KEY (author_id)
        REFERENCES `user` (id),
    CONSTRAINT fk_discussion_comment_parent FOREIGN KEY (parent_comment_id)
        REFERENCES discussion_comment (id) ON DELETE CASCADE,
    CONSTRAINT fk_discussion_comment_removed_by FOREIGN KEY (removed_by)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT chk_discussion_comment_content CHECK (CHAR_LENGTH(TRIM(content)) BETWEEN 1 AND 2000),
    CONSTRAINT chk_discussion_comment_status CHECK (status_int BETWEEN 0 AND 1),
    INDEX idx_discussion_comment_thread (discussion_id, parent_comment_id, created_at),
    INDEX idx_discussion_comment_author_created (author_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS discussion_comment_vote (
    comment_id INT NOT NULL,
    user_id INT NOT NULL,
    vote_value TINYINT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL,
    CONSTRAINT pk_discussion_comment_vote PRIMARY KEY (comment_id, user_id),
    CONSTRAINT fk_discussion_comment_vote_comment FOREIGN KEY (comment_id)
        REFERENCES discussion_comment (id) ON DELETE CASCADE,
    CONSTRAINT fk_discussion_comment_vote_user FOREIGN KEY (user_id)
        REFERENCES `user` (id) ON DELETE CASCADE,
    CONSTRAINT chk_discussion_comment_vote_value CHECK (vote_value IN (-1, 1)),
    INDEX idx_discussion_comment_vote_user_created (user_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS content_report (
    id BIGINT NOT NULL AUTO_INCREMENT,
    reporter_id INT NOT NULL,
    discussion_id INT NULL,
    comment_id INT NULL,
    reason VARCHAR(80) NOT NULL,
    details VARCHAR(1000) NULL,
    status_int TINYINT UNSIGNED NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reviewed_by INT NULL,
    reviewed_at DATETIME NULL,
    resolution_note VARCHAR(500) NULL,
    CONSTRAINT pk_content_report PRIMARY KEY (id),
    CONSTRAINT fk_content_report_reporter FOREIGN KEY (reporter_id)
        REFERENCES `user` (id),
    CONSTRAINT fk_content_report_discussion FOREIGN KEY (discussion_id)
        REFERENCES discussion (id) ON DELETE CASCADE,
    CONSTRAINT fk_content_report_comment FOREIGN KEY (comment_id)
        REFERENCES discussion_comment (id) ON DELETE CASCADE,
    CONSTRAINT fk_content_report_reviewer FOREIGN KEY (reviewed_by)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT chk_content_report_target CHECK (
        (discussion_id IS NOT NULL AND comment_id IS NULL)
        OR (discussion_id IS NULL AND comment_id IS NOT NULL)
    ),
    CONSTRAINT chk_content_report_status CHECK (status_int BETWEEN 0 AND 2),
    CONSTRAINT chk_content_report_reason CHECK (CHAR_LENGTH(TRIM(reason)) BETWEEN 3 AND 80),
    CONSTRAINT uq_content_report_discussion UNIQUE (reporter_id, discussion_id),
    CONSTRAINT uq_content_report_comment UNIQUE (reporter_id, comment_id),
    INDEX idx_content_report_queue (status_int, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS moderation_action (
    id BIGINT NOT NULL AUTO_INCREMENT,
    moderator_id INT NULL,
    action_type TINYINT UNSIGNED NOT NULL,
    target_type TINYINT UNSIGNED NOT NULL,
    target_id BIGINT NOT NULL,
    reason VARCHAR(500) NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_moderation_action PRIMARY KEY (id),
    CONSTRAINT fk_moderation_action_moderator FOREIGN KEY (moderator_id)
        REFERENCES `user` (id) ON DELETE SET NULL,
    CONSTRAINT chk_moderation_action_type CHECK (action_type BETWEEN 1 AND 11),
    CONSTRAINT chk_moderation_target_type CHECK (target_type BETWEEN 1 AND 4),
    INDEX idx_moderation_action_target (target_type, target_id, created_at),
    INDEX idx_moderation_action_moderator (moderator_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
