# Azure App Service deployment guide

## What was prepared
- App startup now binds to the App Service port from `PORT` or `WEBSITES_PORT`.
- Forwarded headers are enabled for Azure’s reverse proxy.
- A Linux startup script was added at `startup.sh`.
- The app is ready for `dotnet publish -c Release` deployment.

## App Service configuration
1. Create a Linux App Service running .NET 8.
2. Set the startup command to:
   ```bash
   /home/site/wwwroot/startup.sh
   ```
3. Configure the following application settings:
   - `ConnectionStrings__DefaultConnection`: your Azure PostgreSQL connection string
   - `Jwt__Key`: a strong secret value
   - `Jwt__Issuer`: your issuer value
   - `Jwt__Audience`: your audience value

## Publish and deploy
```bash
dotnet publish -c Release
```

Then deploy the published output from `bin/Release/net8.0/publish/` to the App Service.
