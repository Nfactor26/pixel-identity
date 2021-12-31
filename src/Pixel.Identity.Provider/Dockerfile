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
RUN dotnet restore "src/Pixel.Identity.Provider/Pixel.Identity.Provider.csproj"
COPY . .
WORKDIR "/src/src/Pixel.Identity.Provider"
RUN dotnet build "Pixel.Identity.Provider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pixel.Identity.Provider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.Identity.Provider.dll"]