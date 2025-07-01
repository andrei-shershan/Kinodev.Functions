# Kinodev Functions - Docker Setup

This Azure Functions project is configured to run in Docker with Traefik support.

## Prerequisites

- Docker Desktop
- Traefik running with a `web` network
- Azurite (Azure Storage Emulator) for local development

## Quick Start

### Option 1: Using PowerShell Helper Script

```powershell
# Build and run the container
.\docker-helper.ps1

# Or specify an action
.\docker-helper.ps1 build
.\docker-helper.ps1 run
.\docker-helper.ps1 compose-up
.\docker-helper.ps1 clean
```

### Option 2: Using Docker Compose

```powershell
# Start all services (including Azurite)
docker-compose up --build

# Stop all services
docker-compose down
```

### Option 3: Manual Docker Commands

```powershell
# Build the image
docker build -t kinodev-functions .

# Run the container
docker run -p 7071:80 --name kinodev-functions-dev `
  -e AzureWebJobsStorage=UseDevelopmentStorage=true `
  -e FUNCTIONS_WORKER_RUNTIME=dotnet-isolated `
  -e FUNCTIONS_EXTENSION_VERSION=~4 `
  kinodev-functions
```

## Traefik Configuration

The docker-compose.yml includes Traefik labels for routing:

- **Host**: `functions.kinodev.localhost`
- **Port**: 80 (internal container port)
- **Network**: `web` (external Traefik network)

Make sure your Traefik instance is configured to use the `web` network and has the appropriate entry points configured.

## Integration with Existing Docker Compose

To integrate with your existing Docker Compose setup, use the service definition:

```yaml
kinodev-functions:
  build: 
    context: "E:/Education/KinoDev/KinoDev.Functions"
    dockerfile: "E:/Education/KinoDev/KinoDev.Functions/Dockerfile"
  container_name: kinodev-functions
  environment:
    - AzureWebJobsStorage=UseDevelopmentStorage=true
    - FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
    - FUNCTIONS_EXTENSION_VERSION=~4
  labels:
    - "traefik.enable=true"
    - "traefik.http.routers.kinodev-functions.rule=Host(`functions.kinodev.localhost`)"
    - "traefik.http.routers.kinodev-functions.entrypoints=web"
    - "traefik.http.services.kinodev-functions.loadbalancer.server.port=80"
  networks:
    - web
  depends_on:
    - azurite   
```

## Testing

Once running, you can test the HelloWorld function:

- **Local**: http://localhost:7071/api/HelloWorld
- **Traefik**: http://functions.kinodev.localhost/api/HelloWorld

## Environment Variables

- `AzureWebJobsStorage`: Connection string for Azure Storage (uses Azurite for local development)
- `FUNCTIONS_WORKER_RUNTIME`: Set to `dotnet-isolated` for .NET 8 isolated worker
- `FUNCTIONS_EXTENSION_VERSION`: Azure Functions runtime version (~4)

## Troubleshooting

1. **Container won't start**: Check Docker logs with `docker logs kinodev-functions`
2. **Traefik routing issues**: Verify the `web` network exists and Traefik is connected to it
3. **Storage issues**: Ensure Azurite is running and accessible
4. **Port conflicts**: Change the host port in docker-compose.yml if 7071 is in use

## File Structure

```
Kinodev.Functions/
├── src/
│   ├── HelloWorldFunction.cs
│   ├── Program.cs
│   ├── host.json
│   ├── local.settings.json
│   ├── docker.settings.json
│   └── Kinodev.Functions.csproj
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── docker-helper.ps1
└── README.md
```
