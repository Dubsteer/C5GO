-- Run this after community-foundation.sql.
-- Replace the value below with the exact username of the single platform owner.

SET @owner_username = 'CHANGE_ME';

CREATE TEMPORARY TABLE c5go_owner_assertion (
    owner_count INT NOT NULL CHECK (owner_count = 1)
);

INSERT INTO c5go_owner_assertion
SELECT COUNT(*)
FROM `user` target
WHERE target.username = @owner_username
  AND target.email_confirmed = 1
  AND NOT EXISTS (
      SELECT 1
      FROM user_role existing_owner
      WHERE existing_owner.role_id = 4
        AND existing_owner.user_id <> target.id
  );

DROP TEMPORARY TABLE c5go_owner_assertion;

INSERT IGNORE INTO user_role
    (user_id, role_id, assigned_by, assigned_at, reason)
SELECT id, 4, NULL, UTC_TIMESTAMP(), 'Initial platform owner'
FROM `user`
WHERE username = @owner_username;

INSERT INTO role_assignment_audit
    (user_id, role_id, action_type, performed_by, reason, created_at)
SELECT id, 4, 0, NULL, 'Initial platform owner', UTC_TIMESTAMP()
FROM `user`
WHERE username = @owner_username
  AND NOT EXISTS (
      SELECT 1
      FROM role_assignment_audit audit
      WHERE audit.user_id = `user`.id
        AND audit.role_id = 4
        AND audit.action_type = 0
  );
