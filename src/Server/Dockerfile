#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Server/Pixel.Identity.Provider.csproj", "src/Server/"]
COPY ["src/Client/Pixel.Identity.UI.Client.csproj", "src/Client/"]
COPY ["src/Shared/Pixel.Identity.Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/Server/Pixel.Identity.Provider.csproj"
COPY . .
WORKDIR "/src/src/Server"
RUN dotnet build "Pixel.Identity.Provider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pixel.Identity.Provider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.Identity.Provider.dll"]