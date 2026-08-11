# Database hardening

The read-only audit was run against the local development database on 11 August 2026. It found no orphaned records and no duplicate tournament applications or team join requests. Status values, scores and notification fields were valid.

The identity data is not ready for unique database constraints. The audit found:

- 7 duplicate username groups;
- 8 duplicate email groups;
- 1 duplicate non-empty SteamID group;
- 46 invalid legacy SteamID values;
- 1 legacy empty or zero SteamID value.

No users or other records were changed while collecting these results.

## Migration order

1. Create and verify a fresh database backup.
2. Run `database-security-audit.sql` and confirm that the phase 1 checks return zero problems.
3. Run `database-hardening-phase-1.sql` once.
4. Start the application and test registration, login, tournaments, teams, comments and notifications.
5. Review duplicate accounts and invalid SteamIDs manually. Do not guess which account belongs to a real user and do not delete accounts only to make the audit pass.
6. Run the audit again. When username, email and SteamID problems are all zero, run `database-hardening-phase-2-identities.sql` once.
7. Run the full automated test suite and repeat the application smoke test.

Phase 1 aligns the schema with the application, prevents duplicate applications and join requests, adds the missing notification relationship and query index, protects email verification tokens, and enforces valid tournament and match values.

Phase 2 normalizes identity values and makes usernames, email addresses and SteamIDs unique at the database level. It intentionally fails before changing the schema while duplicate or invalid identity data still exists.

The unused `match`, `players` and `tournament_teams` tables appear to be legacy tables. They are not removed by these migrations because dropping tables is destructive. They can be archived and removed later only after a backup and a final usage check.

For a new empty database, use `database/schema.sql` instead of the two migration files. It creates only the tables used by the website and already contains both hardening phases. It does not create a database, users or sample data.
