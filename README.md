## Visual Studio Code

tasks.json
```
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build-product",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/product/src/Product.WebApi/Product.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "group": {
                "kind": "build",
                "isDefault": true
            },
            "label": "build-lookup",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/lookup/Lookup.WebApi/Lookup.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "build-validator",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/validator/Validator.Worker/Validator.Worker.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

launch.json
```
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "product",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-product",
            "program": "${workspaceFolder}/product/src/Product.WebApi/bin/Debug/net6.0/Product.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/product/src/Product.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "lookup",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-lookup",
            "program": "${workspaceFolder}/src/services/lookup/Lookup.WebApi/bin/Debug/net6.0/Lookup.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/lookup/Lookup.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "validator",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-validator",
            "program": "${workspaceFolder}/src/services/validator/Validator.Worker/bin/Debug/net6.0/Validator.Worker.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/validator/Validator.Worker",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    ]
}
```

### Migration

In case schema was changed during development, run these scripts to make database up to date

```
$ cd src/Product.Domain
$ dotnet ef migrations add <MigrationName> -c ProductContext -o Persistence/Migrations/Postgre -s ../Product.WebApi
```