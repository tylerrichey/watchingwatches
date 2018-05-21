FROM microsoft/aspnetcore-build AS builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /source
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish --output /app/ --configuration Release

FROM microsoft/aspnetcore:2
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "WatchingWatches.dll"]