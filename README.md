# Pixel Identity
Pixel Identity is a blazor based UI on top of https://github.com/openiddict/openiddict-core and Asp.Net Core Identity with an aim to **quickly setup an OpenID Connect 
service** for your applications. Pixel Identity provides a web based UI to manage Users and Roles associated with Asp.Net Core Identity as well as entities like
OpenIddictApplicationDescriptor and OpenIddictScopeDescriptor required by https://github.com/openiddict/openiddict-core. 
It also provides support for working with **multiple databases e.g. MongoDb, Postgres, SQL Server** using plugins.

## Getting started

You can run Pixely Identity behind a Reverse proxy with SSL termination with any supported database
- Use with docker. 
- Standalone executable on any OS supported by .net core
