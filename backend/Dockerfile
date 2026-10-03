# syntax=docker/dockerfile:1

# Build with the .NET SDK; this stage never ships in the runtime image.
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy only dependency manifests first so package restore is cached until they change.
COPY CampusConnect.slnx ./
COPY src/Api/Api.csproj src/Api/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/
COPY src/Modules/Auth/Auth.csproj src/Modules/Auth/
COPY src/Modules/Communities/Communities.csproj src/Modules/Communities/
COPY src/Modules/Events/Events.csproj src/Modules/Events/
COPY src/Modules/Resources/Resources.csproj src/Modules/Resources/
COPY src/Modules/Users/Users.csproj src/Modules/Users/
RUN dotnet restore src/Api/Api.csproj

# Publish a framework-dependent, optimized release build for the smaller ASP.NET runtime image.
COPY src/ src/
RUN dotnet publish src/Api/Api.csproj --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

# Keep the final image to the ASP.NET runtime and the published application only.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Run the API with an unprivileged account; 8080 does not require root privileges.
RUN addgroup --system --gid 10001 appgroup \
    && adduser --system --uid 10001 --ingroup appgroup appuser

COPY --from=build --chown=appuser:appgroup /app/publish ./

# .NET containers conventionally listen on 8080; infrastructure supplies secrets and connection strings.
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]
