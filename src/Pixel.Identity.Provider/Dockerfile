#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

#identity projects
COPY ["src/Pixel.Identity.Core/Pixel.Identity.Core.csproj", "src/Pixel.Identity.Core/"]
COPY ["src/Pixel.Identity.Provider/Pixel.Identity.Provider.csproj", "src/Pixel.Identity.Provider/"]
COPY ["src/Pixel.Identity.Shared/Pixel.Identity.Shared.csproj", "src/Pixel.Identity.Shared/"]
COPY ["src/Pixel.Identity.UI.Client/Pixel.Identity.UI.Client.csproj", "src/Pixel.Identity.UI.Client/"]

#plugin projects
COPY ["src/Pixel.Identity.Messenger.Email/Pixel.Identity.Messenger.Email.csproj", "src/Pixel.Identity.Messenger.Email/"]
COPY ["src/Pixel.Identity.Messenger.Console/Pixel.Identity.Messenger.Console.csproj", "src/Pixel.Identity.Messenger.Console/"]
COPY ["src/Pixel.Identity.Store.Sql.Shared/Pixel.Identity.Store.Sql.Shared.csproj", "src/Pixel.Identity.Store.Sql.Shared/"]
COPY ["src/Pixel.Identity.Store.SqlServer/Pixel.Identity.Store.SqlServer.csproj", "src/Pixel.Identity.Store.SqlServer/"]
COPY ["src/Pixel.Identity.Store.PostgreSQL/Pixel.Identity.Store.PostgreSQL.csproj", "src/Pixel.Identity.Store.PostgreSQL/"]
COPY ["src/Pixel.Identity.Store.Mongo/Pixel.Identity.Store.Mongo.csproj", "src/Pixel.Identity.Store.Mongo/"]

RUN dotnet restore "src/Pixel.Identity.Messenger.Email/Pixel.Identity.Messenger.Email.csproj"
RUN dotnet restore "src/Pixel.Identity.Messenger.Console/Pixel.Identity.Messenger.Console.csproj"
RUN dotnet restore "src/Pixel.Identity.Provider/Pixel.Identity.Provider.csproj"
RUN dotnet restore "src/Pixel.Identity.Store.Mongo/Pixel.Identity.Store.Mongo.csproj"
RUN dotnet restore "src/Pixel.Identity.Store.SqlServer/Pixel.Identity.Store.SqlServer.csproj"
RUN dotnet restore "src/Pixel.Identity.Store.PostgreSQL/Pixel.Identity.Store.PostgreSQL.csproj"

COPY . .
WORKDIR "/src/src/Pixel.Identity.Provider"

FROM build AS publish
RUN dotnet publish "Pixel.Identity.Provider.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.Identity.Provider.dll"]