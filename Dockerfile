FROM microsoft/dotnet:2.1-sdk AS builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /source
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish --output /app/ --configuration Release -r linux-x64

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "WatchingWatches.dll"]
