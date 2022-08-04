## Visual Studio Code

Project initiation
```
$ dotnet new classlib -o <module>
$ dotnet new webapi -minimal -o <module>.WebApi
```

tasks.json
```
{
    "version": "2.0.0",
    "tasks": [
        {
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
            "problemMatcher": "$msCompile",
            "group": "build"
        },
        {
            "label": "build-store",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/store/Store.WebApi/Store.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "group": "build",
            "label": "build-cart",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/cart/Cart.WebApi/Cart.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "build-order",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/order/Order.WebApi/Order.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile",
            "group": "build"
        },
        {
            "label": "build-payment",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/payment/Payment.WebApi/Payment.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile",
            "group": "build"
        },
        {
            "label": "build-shipment",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/shipment/Shipment.WebApi/Shipment.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        },
        {
            "label": "build-search",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/services/search/Search.WebApi/Search.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        },
        {
            "label": "build-gateway",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/src/gateway/Gateway.WebApi/Gateway.WebApi.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary",
                "--no-restore"
            ],
            "problemMatcher": "$msCompile",
            "group": "build"
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
            "name": "store",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-store",
            "program": "${workspaceFolder}/src/services/store/Store.WebApi/bin/Debug/net6.0/Store.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/store/Store.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "cart",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-cart",
            "program": "${workspaceFolder}/src/services/cart/Cart.WebApi/bin/Debug/net6.0/Cart.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/cart/Cart.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "order",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-order",
            "program": "${workspaceFolder}/src/services/order/Order.WebApi/bin/Debug/net6.0/Order.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/order/Order.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "payment",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-payment",
            "program": "${workspaceFolder}/src/services/payment/Payment.WebApi/bin/Debug/net6.0/Payment.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/payment/Payment.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "shipment",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-shipment",
            "program": "${workspaceFolder}/src/services/shipment/Shipment.WebApi/bin/Debug/net6.0/Shipment.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/shipment/Shipment.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "search",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-search",
            "program": "${workspaceFolder}/src/services/search/Search.WebApi/bin/Debug/net6.0/Search.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/services/search/Search.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        },
        {
            "name": "gateway",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-gateway",
            "program": "${workspaceFolder}/src/gateway/Gateway.WebApi/bin/Debug/net6.0/Gateway.WebApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/gateway/Gateway.WebApi",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    ],
    "compounds": [
        {
            "name": "everything",
            "configurations": [
                "lookup",
                "store",
                "cart",
                "order",
                "payment",
                "shipment",
                "search",
                "gateway"
            ]
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