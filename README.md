### Migration

In case schema was changed during development, run these scripts to make database up to date

```
$ cd src/Product.Domain
$ dotnet ef migrations add <MigrationName> -c ProductContext -o Migration/Postgre/Product -s ../Product.WebApi
```