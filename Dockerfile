FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build-env
WORKDIR /app

COPY ./*sln ./

COPY ./OpenFTTH.TileDataExtractor/*.csproj ./OpenFTTH.TileDataExtractor/

RUN dotnet restore --packages ./packages

COPY . ./
WORKDIR /app/OpenFTTH.TileDataExtractor
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app

COPY --from=build-env /app/OpenFTTH.TileDataExtractor/out .
ENTRYPOINT ["dotnet", "OpenFTTH.TileDataExtractor.dll"]