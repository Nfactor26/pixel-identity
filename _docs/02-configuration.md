---
title: "Configuration"
permalink: /docs/configuration/
---

### General settings

| Option      | Description | Is Required ?|
| ----------- | ----------- | -------      |
|  InitAdminUser          | User name for the initial admin user that is created.                                                             | Required |
|  InitAdminUserPass      | Password for the initial adming user that is created.                                                             | Required |
|  IdentityHost           | Url over which pixel-identity is accessed. This is required to auto configure Pixel Identity UI with OpenIddict.  | Required |
|  AllowedOrigins         | Allowed origins for CORS.                                                                                         | Optional |
|  AllowUserRegistration  | Allow users to register an account. This is suitable for public websites. Default value is true                   | Optional |
|  AutoMigrate            | Allow EntityFramework migrations to be auto applied. See Database Plugin options section for more details         | Optional | 

- appsettings.json

        "InitAdminUser": "admin@pixel.com",
        "InitAdminUserPass": "Admi9@pixel",
        "IdentityHost": "http://localhost:44382/pauth",
        "AllowedOrigins": "http://localhost:44382",
        "AllowUserRegistration": true,
        "AutoMigrate": false

- environment variable or .env files for docker
 
        InitAdminUser=admin@pixel.com
        InitAdminUserPass=Admi9@pixel
        AllowedOrigins=https://pixel.docker.localhost
        IdentityHost=https://pixel.docker.localhost/pauth
        AllowUserRegistration=true
        AutoMigrate=false

### Certificates

[OpenIddict](https://github.com/openiddict/openiddict-core) requires two certificates for [encryption and signing](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html)


| Option      | Description | Is Required ?|
| ----------- | ----------- | -------      |
|  EncryptionCertificatePath     | Aboslute path for encryption certificate. A development certificate is auto generated if not specified. You should provide a certificate for production enviornment. | Optional |
|  EncryptionCertificateKey   | Password for encryption ceritificate if any provided while generating certificate.  | Optional |
|  SigningCertificatePath   | Absolute path for signing certificate. A development certificate is auto generated if not specified. You should provide a certificate for production environment.  | Optional |
|  SigningCertificateKey   | Password for signing certificate if any provided while generating certificate.  | Optional |

- appsettings.json

        "Identity": {
            "Certificates": {
                "EncryptionCertificatePath": ".....\\.certificates\\identity-encryption.pfx",
                "EncryptionCertificateKey": "",
                "SigningCertificatePath": ".......\\.certificates\\identity-signing.pfx",
                "SigningCertificateKey": ""
            }
        }

- environment variable or .env files for docker  

        Identity__Certificates__EncryptionCertificatePath=.....\\.certificates\\identity-encryption.pfx
        Identity__Certificates__EncryptionCertificateKey=""
        Identity__Certificates__SigningCertificatePath=.....\\.certificates\\identity-signing.pfx
        Identity__Certificates__SigningCertificateKey=""


### Database plugin Options

Pixel Identity provides support for MongoDB, Postgres SQL and Microsoft SQL server out of the box. You can configure the desired database plugin as shown below.
Use Pixel.Identity.Store.Mongo or Pixel.Identity.Store.SqlServer or Pixel.Identity.Store.PostgreSQL as Name value.

- appsettings.json

        "Plugins": {
            "Collection": [      
                {
                    "Type": "DbStore",
                    "Path": "Plugins\\DbStore",
                    "Name": "Pixel.Identity.Store.Mongo"
                }
            ]
        }

    ConnectionString for SqlServer 
        
        "ConnectionStrings": {
            "SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=pixel-identity-db;Trusted_Connection=True;MultipleActiveResultSets=true",       
        }
    
    ConnectionString for Postgres SQL
        
        "ConnectionStrings": {       
            "PostgreServerConnection": "User ID=postgresadmin;Password=postgrespass;Server=postgres;Port=5432;Database=pixel_identity_db;"
        }

    ConnectionString for MongoDB
        
        "MongoDbSettings": {
            "ConnectionString": "mongodb://localhost:27017",
            "DatabaseName": "pixel-identity-db"
        }    

- environment variable or .env files for docker
    
        Plugins__Collection__n__Type=DbStore
        Plugins__Collection__n__Path=Plugins/DbStore
        Plugins__Collection__n__Name=Pixel.Identity.Store.Mongo

    where n = 0, 1, 2 .... depending on their position in Plugin collection

    ConnectionString for SqlServer 
        
        ConnectionStrings__SqlServerConnection=Server=(localdb)\\mssqllocaldb;Database=pixel-identity-db;Trusted_Connection=True;MultipleActiveResultSets=true       
   
    ConnectionString for Postgres SQL

        ConnectionStrings__PostgreServerConnection=User ID=postgresadmin;Password=postgrespass;Server=postgres;Port=5432;Database=pixel_identity_db

    ConnectionString for MongoDB

        MongoDbSettings__ConnectionString=mongodb://localhost:27017
        MongoDbSettings__DatabaseName=pixel-identity-db 
    
  SQL based plugins use entity framework migration feature starting v3.0 of pixel-identity. Please use provided scripts with the release to create required database.
  Alternatively, a feature flag 'AutoMigrate' can be set to true to auto apply migration. This should be fine for quickly spinning up a docker or trying locally in dev environment.
  Recommened approach is to use provided SQL scripts for production usage.

  ### Email Sender plugin

  An email sender plugin is required by Pixel Identity to send mail to users e.g. for email confirmation and password reset links.
  Pixel Identity comes with two inbuilt plugins that implement the IEmailSender interface.
  - Pixel.Identity.Messenger.Console  -- Dummy plugin and configured by default. Functionalities like email confirmation and password resets won't work when using this.
  - Pixel.Identity.Messenger.Email  -- Capable of sending mails using SMTP. Additional configuration is required.

  
  Configuring the Email Sender plugin to use :
  
  - appsettings.json

        "Plugins": {
            "Collection": [
                {
                    "Type": "EmailSender",
                    "Path": "Plugins\\Messenger",
                    "Name": "Pixel.Identity.Messenger.Console"
                }      
           ]
        }

      Additional configuration required if using **Pixel.Identity.Messenger.Email** plugin for detail of SMPT server used for sending emails.
      For development, https://ethereal.email/ account is used for SMTP configuration.
      
        "SMTP":
        {
            "Host": "smtp.ethereal.email",
            "Port": 587,
            "UserName": "",
            "Password": "",
            "From": ""
        }

  - environment variable or .env files for docker
      
        Plugins__Collection__n__Type=EmailSender
        Plugins__Collection__n__Path=Plugins/Messenger
        Plugins__Collection__n__Name=Pixel.Identity.Messenger.Console
 
      where n = 0, 1, 2 .... depending on their position in Plugin collection

      Additional configuration required if using **Pixel.Identity.Messenger.Email** plugin for detail of SMPT server used for sending emails.
        
        SMTP__HOST=smtp.ethereal.email
        SMTP__PORT=587
        SMTP_UserName=
        SMTP_Password=
        SMTP_From=

### Asp.Net Identity Options

Below Options can be configured for [Asp.Net Identity ](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-6.0)
All these options are optional and will use default value if not configured.


| Option      | Description | Default Value|
| ----------- | ----------- | ------- |
|  SignIn.RequireConfirmedPhoneNumber     | Requires a confirmed phone number to sign in.      | false |
|  SignIn.RequireConfirmedEmail           | Requires a confirmed email to sign in.             | false |
|  SignIn.RequireConfirmedAccount         | Indicates whether a confirmed account is required to sign in.      |  false      |
|  User.AllowedUserNameCharacters         | Allowed characters in the username.      |  abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+      |
|  User.RequireUniqueEmail                | Requires each user to have a unique email.    |  false      |
|  Password.RequiredLength                | The minimum length of the password.      |  6      |
|  Password.RequiredUniqueChars           | Requires the number of distinct characters in the password.    |   1     |
|  Password.RequireNonAlphanumeric        | Requires a non-alphanumeric character in the password.      |   true     |
|  Password.RequireLowercase              | Requires a lowercase character in the password.    |   true     |
|  Password.RequireUppercase              | Requires an uppercase character in the password.    |  true      |
|  Password.RequireDigit                  | Requires a number between 0-9 in the password.     |   true     |
|  Lockout.AllowedForNewUsers             | Determines if a new user can be locked out.     |   true     |
|  Lockout.MaxFailedAccessAttempts        | The amount of time a user is locked out when a lockout occurs.       |    5 minutes    |
|  Lockout.DefaultLockoutTimeSpan         | The number of failed access attempts until a user is locked out, if lockout is enabled.     |   5    |

- appsettings.json

        "IdentityOptions": {
            "SignIn": {
                "RequireConfirmedAccount": false
            }
         }

- environment variable or .env files for docker
        
        IdentityOptions_SignIn_RequireConfirmedAccount=false
