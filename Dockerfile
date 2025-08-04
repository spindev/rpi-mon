# Multi-stage Docker build optimized for size and security
# Use the official .NET SDK Alpine image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set the working directory
WORKDIR /app

# Copy only the project file first for better layer caching
COPY ["RpiMon/RpiMon.csproj", "./"]
RUN dotnet restore --verbosity minimal

# Copy the rest of the source code
COPY RpiMon/ .

# Build and publish the application with optimizations
RUN dotnet publish -c Release -o out \
    --no-restore \
    --verbosity minimal

# Runtime stage with minimal Alpine image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install only essential system dependencies for hardware monitoring
RUN apk add --no-cache \
    procps \
    && rm -rf /var/cache/apk/*

# Create a non-root user for better security
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup

# Set the working directory
WORKDIR /app

# Copy the published application from build stage
COPY --from=build /app/out .

# Set proper ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose the application port
EXPOSE 5000

# Configure environment variables for optimization
ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

# Run the application
ENTRYPOINT ["dotnet", "RpiMon.dll"]