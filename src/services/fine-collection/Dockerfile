FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src

COPY . .
RUN dotnet publish \
    -c Release \
    -o /app/publish \
    -r linux-musl-x64 \
    --sc true \
    /p:PublishTrimmed=true \
    /p:PublishReadyToRun=true \
    /p:PublishSingleFile=true \
    /p:PublishAOT=true

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine-amd64 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["./FineCollection.Worker"]