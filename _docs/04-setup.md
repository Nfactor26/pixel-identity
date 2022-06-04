---
title: "Up and Running"
permalink: /docs/setup/
---

### Setup with docker

Download and extract the docker-compose.zip file from [releases](https://github.com/Nfactor26/pixel-identity/releases) page from github repo.
Make sure to unblock the zip file.Generate all the three required certificates as described in the [certificates guide](/docs/certificates) and put in the **.certificates** folder.Go through the [configurations](/docs/configuration) and modify as required. 

Execute below commands from docker-compose folder after starting docker. 

    docker volume create postgre-pixel-identiy-store  # For Postgres SQL 
    docker volume create mongo-pixel-identiy-store    # For MongoDB

    docker network create --driver bridge pixel-network
    
    docker compose -f docker-compose-traefik-postgres.yml up -d  # For Postgres SQL 
    docker compose -f docker-compose-traefik-mongo.yml up -d  # For MongoDB

Execute below commands from docker-compose folder to stop.

    docker compose -f docker-compose-traefik-postgres.yml down # For Postgres SQL
    docker compose -f docker-compose-traefik-mongo.yml down  # For MongoDB 

Navigate to https://pixel.docker.localhost/pauth once the containers are up and running.
Login with values configured for  InitAdminUser and InitAdminUserPass configuration keys to get started. You can change password once logged in.

#### Reverse-Proxy

While the service can be exposed directly (with Kestrel), it is recommended to use it behind a reverse proxy with TLS termination. See [guidance](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/when-to-use-a-reverse-proxy?view=aspnetcore-6.0) on when to use Kestrel with a reverse proxy.

Default setup uses [Traefik](https://traefik.io/traefik/) for reverse-proxy. Traefik will automatically pick required certificate pixel-cert.pem (certificate file) and pixel-key.pem (key file) from .certificates folder. See .traefik->traefik.yml configuration file to modify this.
You can choose to use a reverse-proxy of your choice e.g. nginx, yarp, etc.

#### Database

Docker compose files are provided to use either MongoDB or Postgres SQL as the backend with Traefik acting as reverse proxy.
Latest version of these database will be pulled and started inside docker. Database is not exposed over any port.
- docker-compose-traefik-mongo.yml to use MongoDB  
    Modify environment varialbes as required in docker-compose file:
        MONGO_INITDB_ROOT_USERNAME: mongoadmin
        MONGO_INITDB_ROOT_PASSWORD: mongopass    
- docker-compose-traefik-postgres.yml to use Postgres SQL
    Modify environment variables as required in docker-compose file:
        POSTGRES_USER: postgresadmin
        POSTGRES_PASSWORD: postgrespass
        POSTGRES_DB: pixel_identity_db

#### Pixel Identity Service

Pixel Identity service is already configured with defaults in docker-compose-*.yml files. Modify the image version if required.
Check [dockerhub](https://hub.docker.com/repository/docker/nfactor26/pixel-identity) for available versions.

    image: nfactor26/pixel-identity:v1.1.0

Service is configured to run on *http* and not exposed on any port. It is assumed that it will be accessed via a reverse-proxy with SSL termination.
It requires identity-encryption.pfx and identity-signing.pfx certificates to be available in .certificates file which are required by 
[OpenIddict for encryption and signing](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html).

### Setup with Windows or Linux

Download and extract pixel-identity-windows.zip or pixel-identity-ubuntu.zip file from [releases](https://github.com/Nfactor26/pixel-identity/releases) page from github repo. Make sure to unblock the zip files. Add required configuration in appsettings.json. Make sure your database is up and running. 

Ensure that environment variable [ASPNETCORE_ENVIRONMENT](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0) is correctly set to
Development or Staging or Production.Launch Pixel Identity service to listen on https. The recommendation however is to use it behind a reverse-proxy with SSL termination.
Make sure you use the same host configured for IdentityHost configuration for --urls parameter e.g. if IdentityHost is https://localhost:44382/pauth you need to pass
--urls=https://localhost:44382.
   
    dotnet Pixel.Identity.Provider.dll --urls=https://localhost:44382/ 

Navigate to https://localhost:44382/pauth displayed on console. Assumption here is that dotnet development certificate is auto generated.
For development enviornmet, https://ethereal.email/ account is used for SMTP configuration.
 

- Example appsettings.Production.json with MongoDB and SMTP based email sender

        {
            "Serilog": 
            {
                "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
                "MinimumLevel":
                {
                    "Default": "Information",
                    "Override": 
                    {
                        "Microsoft": "Warning",
                        "Microsoft.Hosting.Lifetime": "Information"
                    }
                },
                "WriteTo": [
                    {
                        "Name": "Console" 
                    },
                    {
                        "Name": "File",
                        "Args": { "path": "logs\\pixel-identity-log-.txt" }
                    }
                ],  
                "Enrich": [ "FromLogContext" ]
            },
            "Plugins": 
            {
                "Collection": [
                {
                    "Type": "EmailSender",
                    "Path": "Plugins\\Messenger",
                    "Name": "Pixel.Identity.Messenger.Email"
                },
                {
                    "Type": "DbStore",
                    "Path": "Plugins\\DbStore",
                    "Name": "Pixel.Identity.Store.Mongo"
                }
                ]
            },
            "IdentityOptions": {
                "SignIn": {
                    "RequireConfirmedAccount": true
                }
            },
            "Identity": {
                "Certificates": {
                    "EncryptionCertificatePath": ".....\\.certificates\\identity-encryption.pfx",
                    "EncryptionCertificateKey": "",
                    "SigningCertificatePath": ".....\\.certificates\\identity-signing.pfx",
                    "SigningCertificateKey": ""
                }
            },
            "SMTP":
            {
                "Host": "",
                "Port": 587,
                "UserName": "",
                "Password": "",
                "From": ""
            },
            "MongoDbSettings": {
                "ConnectionString": "mongodb://mongoadmin:mongopass@mongo:27017/",
                "DatabaseName": "pixel-identity-db-test"
            },
            "InitAdminUser": "admin@pixel.com",
            "InitAdminUserPass": "Admi9@pixel",
            "AllowedHosts": "http://localhost:44382/pauth"
            "AllowedOrigins": "http://localhost:44382"
        }       


