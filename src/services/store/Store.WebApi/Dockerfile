# Defining base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0.6-alpine3.14 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy from release folder
FROM base AS final
WORKDIR /app
COPY /release .
ENTRYPOINT ["dotnet", "Store.WebApi.dll"]