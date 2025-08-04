# Data Protection Configuration for RpiMon

This application uses targeted logging configuration to suppress data protection warnings that are not relevant for a monitoring application.

## Rationale

The `Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager` warning about unencrypted keys is suppressed because:

1. **Monitoring Application Context**: RpiMon is a system monitoring application that doesn't handle sensitive user authentication data or store personal information that requires data protection.

2. **Stateless Design**: The application is designed to be stateless and doesn't require persistent authentication sessions across container restarts.

3. **Container Environment**: In containerized deployments, data protection keys are ephemeral by design, which is appropriate for this use case.

4. **Security by Design**: The application follows security best practices in other areas (non-root user, read-only filesystem, capability restrictions) while acknowledging that persistent data protection keys are not required for its monitoring functionality.

This approach resolves the startup warnings while maintaining appropriate security posture for a monitoring application.