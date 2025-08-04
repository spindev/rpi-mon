# RPi Monitor

A real-time Raspberry Pi monitoring web application built with .NET Blazor Server and SignalR.

![RPi Monitor Screenshot](https://github.com/user-attachments/assets/c50c4747-c22d-47b8-b5bf-09033a04d9b9)

## Features

- **Real-time System Monitoring**: Live updates via SignalR every 5 seconds
- **System Information Display**: 
  - Operating System details
  - Hardware architecture
  - Raspberry Pi model information
  - RAM size and usage
  - CPU information and usage
  - CPU temperature monitoring
- **Modern UI**: Clean, responsive design with dark/light mode toggle
- **Mobile-First Approach**: Optimized for mobile devices
- **No Authentication Required**: Simple access for home lab environments
- **Docker Support**: Easy deployment with Docker and docker-compose

## Technology Stack

- **.NET 8**: Latest .NET framework
- **Blazor Server**: Server-side rendering with real-time updates
- **SignalR**: Real-time communication for live data updates
- **Bootstrap**: Responsive UI framework
- **Font Awesome**: Modern icons (CDN-based)

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)

### Running Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/spindev/rpi-mon.git
   cd rpi-mon/RpiMon
   ```

2. Build and run the application:
   ```bash
   dotnet build
   dotnet run --urls=http://localhost:5000
   ```

3. Open your browser and navigate to `http://localhost:5000`

### Docker Deployment

The easiest way to deploy RPi Monitor is using Docker:

1. Clone the repository:
   ```bash
   git clone https://github.com/spindev/rpi-mon.git
   cd rpi-mon
   ```

2. Build and run with docker-compose:
   ```bash
   docker-compose up -d
   ```

3. Access the application at `http://your-rpi-ip:5000`

### Manual Docker Build

If you prefer to build the Docker image manually:

```bash
# Build the image
docker build -t rpi-mon .

# Run the container
docker run -d \
  --name rpi-mon \
  -p 5000:5000 \
  -v /proc:/host/proc:ro \
  -v /sys:/host/sys:ro \
  -v /etc:/host/etc:ro \
  --privileged \
  rpi-mon
```

## System Requirements

- **Raspberry Pi**: Any model (Pi 3, Pi 4, Pi 5, etc.)
- **Operating System**: Raspberry Pi OS (Debian-based Linux)
- **Memory**: Minimum 512MB RAM (1GB+ recommended)
- **Docker**: Latest version for containerized deployment

## Features in Detail

### System Information
- Automatically detects Raspberry Pi model
- Shows OS version and architecture
- Displays total RAM and current usage
- CPU model and core count information

### Real-time Monitoring
- CPU usage percentage
- Memory usage with available/total breakdown
- CPU temperature (with color-coded warnings)
- Automatic updates every 5 seconds

### User Interface
- **Dark/Light Mode**: Toggle between themes
- **Mobile Responsive**: Works great on phones and tablets
- **Real-time Status**: Connection indicator shows live data status
- **Clean Design**: Modern card-based layout

## Configuration

The application uses standard ASP.NET Core configuration. You can customize settings in `appsettings.json`:

- **Update Interval**: Modify the background service timing
- **SignalR Settings**: Configure connection timeouts
- **Logging**: Adjust log levels

## Development

### Project Structure

```
RpiMon/
├── Components/           # Blazor components
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── Models/              # Data models
├── Services/            # Business logic services
├── Hubs/               # SignalR hubs
├── wwwroot/            # Static files
└── Program.cs          # Application entry point
```

### Adding New Metrics

To add new system metrics:

1. Update the `SystemInfo` model in `Models/SystemInfo.cs`
2. Add the data collection logic to `Services/SystemInfoService.cs`
3. Update the UI in `Components/Pages/Home.razor`

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Roadmap

- [ ] Historical data charts (CPU, memory, temperature over time)
- [ ] Disk usage monitoring
- [ ] Network interface statistics
- [ ] Process monitoring
- [ ] System service status
- [ ] Email/SMS alerts for critical temperatures
- [ ] Export data functionality
- [ ] Multiple Pi monitoring from single dashboard