SET @schema_name = DATABASE();

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = @schema_name AND table_name = 'matches' AND column_name = 'round_number'
    ),
    'SELECT 1',
    'ALTER TABLE matches ADD COLUMN round_number INT NOT NULL DEFAULT 1 AFTER status_int'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = @schema_name AND table_name = 'matches' AND column_name = 'bracket_position'
    ),
    'SELECT 1',
    'ALTER TABLE matches ADD COLUMN bracket_position INT NOT NULL DEFAULT 1 AFTER round_number'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = @schema_name AND table_name = 'team_matches' AND column_name = 'round_number'
    ),
    'SELECT 1',
    'ALTER TABLE team_matches ADD COLUMN round_number INT NOT NULL DEFAULT 1 AFTER status_int'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = @schema_name AND table_name = 'team_matches' AND column_name = 'bracket_position'
    ),
    'SELECT 1',
    'ALTER TABLE team_matches ADD COLUMN bracket_position INT NOT NULL DEFAULT 1 AFTER round_number'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.statistics
        WHERE table_schema = @schema_name AND table_name = 'matches' AND index_name = 'idx_matches_bracket'
    ),
    'SELECT 1',
    'ALTER TABLE matches ADD INDEX idx_matches_bracket (tournamentId, round_number, bracket_position)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_schema = @schema_name AND table_name = 'matches' AND constraint_name = 'chk_matches_round'
    ),
    'SELECT 1',
    'ALTER TABLE matches ADD CONSTRAINT chk_matches_round CHECK (round_number >= 1)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_schema = @schema_name AND table_name = 'matches' AND constraint_name = 'chk_matches_position'
    ),
    'SELECT 1',
    'ALTER TABLE matches ADD CONSTRAINT chk_matches_position CHECK (bracket_position >= 1)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.statistics
        WHERE table_schema = @schema_name AND table_name = 'team_matches' AND index_name = 'idx_team_matches_bracket'
    ),
    'SELECT 1',
    'ALTER TABLE team_matches ADD INDEX idx_team_matches_bracket (tournamentId, round_number, bracket_position)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_schema = @schema_name AND table_name = 'team_matches' AND constraint_name = 'chk_team_matches_round'
    ),
    'SELECT 1',
    'ALTER TABLE team_matches ADD CONSTRAINT chk_team_matches_round CHECK (round_number >= 1)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;

SET @statement = IF(
    EXISTS(
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_schema = @schema_name AND table_name = 'team_matches' AND constraint_name = 'chk_team_matches_position'
    ),
    'SELECT 1',
    'ALTER TABLE team_matches ADD CONSTRAINT chk_team_matches_position CHECK (bracket_position >= 1)'
);
PREPARE migration_statement FROM @statement;
EXECUTE migration_statement;
DEALLOCATE PREPARE migration_statement;
