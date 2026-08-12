# Community foundation

The Community feature is developed behind the `Features:CommunityEnabled` switch. The default value is `false`, so unfinished pages cannot become active accidentally.

`community-foundation.sql` is an additive migration. It does not remove or rename existing tables and it does not change the current administrator flag. It creates:

- the Owner, Admin, Moderator and Member role hierarchy;
- role assignment history;
- discussion categories, discussions and optional media fields;
- discussion and comment votes;
- comments with one reply level;
- reports and moderation history.

Every existing user receives the Member role. Accounts currently marked as administrators also receive the Admin role. The migration intentionally does not select an Owner because that decision must be made explicitly.

Before applying the migration:

1. Create and verify a fresh database backup.
2. Confirm the server uses MySQL 8.0.16 or newer.
3. Run `community-foundation.sql` once against the development database.
4. Set the exact verified owner username in `community-owner-setup.sql` and run it once. The script stops if a different Owner already exists.
5. Keep `Features:CommunityEnabled` disabled until the repositories and pages are ready.

Enable the feature locally only after both SQL scripts succeed:

```powershell
dotnet user-secrets set "Features:CommunityEnabled" "true" --id "C5GO-Website-Diplomski"
```

Turning the switch back to `false` hides Community and returns authentication to the legacy administrator flag without deleting any Community data. Uploaded files are stored under `Website/wwwroot/Images/community`; production hosting must persist and back up this directory together with the database.
