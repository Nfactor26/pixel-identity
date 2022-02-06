#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Pixel.Identity.Provider/Pixel.Identity.Provider.csproj", "src/Pixel.Identity.Provider/"]
COPY ["src/Pixel.Identity.Shared/Pixel.Identity.Shared.csproj", "src/Pixel.Identity.Shared/"]
COPY ["src/Pixel.Identity.UI.Client/Pixel.Identity.UI.Client.csproj", "src/Pixel.Identity.UI.Client/"]
COPY ["src/Pixel.Identity.Core/Pixel.Identity.Core.csproj", "src/Pixel.Identity.Core/"]
COPY ["src/Pixel.Identity.Store.Sql/Pixel.Identity.Store.Sql.csproj", "src/Pixel.Identity.Store.Sql/"]
COPY ["src/Pixel.Identity.Store.Mongo/Pixel.Identity.Store.Mongo.csproj", "src/Pixel.Identity.Store.Mongo/"]

RUN dotnet restore "src/Pixel.Identity.Provider/Pixel.Identity.Provider.csproj"
RUN dotnet restore "src/Pixel.Identity.Store.Mongo/Pixel.Identity.Store.Mongo.csproj"
RUN dotnet restore "src/Pixel.Identity.Store.Sql/Pixel.Identity.Store.Sql.csproj"

COPY . .
WORKDIR "/src/src/Pixel.Identity.Provider"
RUN dotnet build "Pixel.Identity.Provider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pixel.Identity.Provider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.Identity.Provider.dll"]