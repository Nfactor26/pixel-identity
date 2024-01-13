# Pixel Identity

Pixel Identity is a blazor based UI on top of https://github.com/openiddict/openiddict-core and Asp.Net Core Identity with an aim to **quickly setup an OpenID Connect 
service** for your applications. Pixel Identity provides a web based UI to manage Users and Roles associated with Asp.Net Core Identity as well as entities like
OpenIddictApplicationDescriptor and OpenIddictScopeDescriptor required by https://github.com/openiddict/openiddict-core. 

## Branches

- main : Ongoing dev for migrating to dotnet 8 and openiddict v5.x
- dotnet6 : This is the stable version with dotnet 6 and openiddict v4.x.


## Features

- Support for multiple databases such as MongoDB, Postgres SQL and Microsoft SQL Server.
- Blazor based UI to easily manage users, roles, applications and scopes.
- Extensible design using plugins.
- Host inside docker or standalone on windows/linux

## Getting started

Please see documentation https://nfactor26.github.io/pixel-identity/ to get started.
