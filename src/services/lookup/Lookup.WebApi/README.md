
## Publish

```
$ dotnet publish --runtime alpine-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true -c Release -o ../../../../release/lookup
```