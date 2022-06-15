### Migration

In case schema was changed during development, run these scripts to make database up to date

```
$ cd src/Product.Domain
$ dotnet ef migrations add <MigrationName> -c ProductContext -o Persistence/Migrations/Postgre -s ../Product.WebApi
```