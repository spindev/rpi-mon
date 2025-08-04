# Docker Build Optimizations

This document outlines the optimizations made to the Docker build process for rpi-mon.

## Optimizations Implemented

### 1. .dockerignore File
Created `.dockerignore` to exclude unnecessary files from the build context:
- Git repository and CI/CD files
- Tests and development files
- Documentation files
- Build artifacts and temporary files

**Impact**: Reduces build context size and improves build speed.

### 2. Alpine Linux Base Images
Switched from standard Debian-based images to Alpine Linux:
- Build stage: `mcr.microsoft.com/dotnet/sdk:8.0-alpine`
- Runtime stage: `mcr.microsoft.com/dotnet/aspnet:8.0-alpine`

**Impact**: Significantly smaller image size (Alpine images are typically 5-10x smaller).

### 3. Optimized Layer Caching
Improved Dockerfile structure for better layer caching:
- Copy project file first, then restore dependencies
- Copy source code only after dependency restoration
- Use `--no-restore` flag in publish command

**Impact**: Faster builds when only source code changes.

### 4. Minimal Runtime Dependencies
Reduced runtime dependencies to essentials:
- Only install `procps` package needed for system monitoring
- Clean up package cache with `rm -rf /var/cache/apk/*`

**Impact**: Smaller runtime image and reduced attack surface.

### 5. Security Improvements
Added security enhancements:
- Created non-root user (`appuser`) for running the application
- Set proper file ownership
- Added environment variables for container optimization

**Impact**: Better security posture and container best practices.

### 6. Docker Compose Optimizations
Improved docker-compose.yml for security:
- Replaced `privileged: true` with specific capabilities (`SYS_ADMIN`)
- Added `no-new-privileges` security option
- Made container filesystem read-only
- Added tmpfs for temporary files

**Impact**: Enhanced security while maintaining functionality.

### 7. Build Optimizations
Added .NET-specific optimizations:
- Set `DOTNET_RUNNING_IN_CONTAINER=true`
- Set `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true` for smaller footprint
- Use minimal verbosity for cleaner build output

**Impact**: Optimized runtime performance and reduced image size.

## Expected Improvements

1. **Image Size**: 30-50% reduction in final image size
2. **Build Time**: Faster builds due to better caching and smaller context
3. **Security**: Non-root execution and minimal privileges
4. **Maintenance**: Cleaner build process with better separation of concerns

## Before/After Comparison

### Original Dockerfile Issues:
- Used larger Debian-based images
- No .dockerignore file
- Ran as root user
- Used privileged Docker mode
- Less optimal layer structure

### Optimized Dockerfile Benefits:
- Alpine-based images for minimal size
- Comprehensive .dockerignore
- Non-root user execution
- Specific capabilities instead of privileged mode
- Better layer caching structure