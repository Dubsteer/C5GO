-- Run this once on an existing C5GO database before starting this version.
-- Existing Steam profiles remain private until each user enables public visibility.

ALTER TABLE `user`
    ADD COLUMN show_steam_profile TINYINT(1) NOT NULL DEFAULT 0 AFTER steam_id;
