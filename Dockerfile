FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY SpotifyAPI/*.csproj ./SpotifyAPI/
COPY Model/*csproj ./Model/

RUN dotnet restore

COPY . .
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

COPY SpotifyAPI/Data /app/Data

ENTRYPOINT ["dotnet", "SpotifyAPI.dll"]