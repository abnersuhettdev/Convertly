FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Convertly.sln ./
COPY NuGet.Config ./
COPY src/Convertly.Api/Convertly.Api.csproj src/Convertly.Api/
COPY src/Convertly.Application/Convertly.Application.csproj src/Convertly.Application/
COPY src/Convertly.Domain/Convertly.Domain.csproj src/Convertly.Domain/
COPY src/Convertly.Infrastructure/Convertly.Infrastructure.csproj src/Convertly.Infrastructure/
COPY tests/Convertly.Tests/Convertly.Tests.csproj tests/Convertly.Tests/

RUN dotnet restore Convertly.sln --configfile NuGet.Config

COPY . .
RUN dotnet publish src/Convertly.Api/Convertly.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN DEBIAN_FRONTEND=noninteractive apt-get update \
    && apt-get install -y --no-install-recommends \
        libreoffice \
        libreoffice-writer \
        fonts-dejavu \
        fonts-liberation \
        fonts-freefont-ttf \
        fontconfig \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    Conversion__LibreOfficePath=libreoffice
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "if [ -n \"$PORT\" ] && [ \"$ASPNETCORE_URLS\" = \"http://+:8080\" ]; then export ASPNETCORE_URLS=\"http://+:$PORT\"; fi; exec dotnet Convertly.Api.dll"]
