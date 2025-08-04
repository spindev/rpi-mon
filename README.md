# rpi-mon

A real-time Raspberry Pi monitoring web application that provides live system metrics through a clean, responsive web interface.

![rpi-mon Screenshot](https://github.com/user-attachments/assets/c50c4747-c22d-47b8-b5bf-09033a04d9b9)

## What is rpi-mon?

rpi-mon is a lightweight monitoring solution for Raspberry Pi devices. It displays real-time system information including CPU temperature, memory usage, and hardware details through a modern web dashboard that updates automatically every 5 seconds.

## Features

- **Real-time monitoring** with live updates via web interface
- **System information** display (OS, hardware, model detection)
- **Performance metrics** (CPU temperature, memory usage, CPU load)
- **Mobile-friendly** responsive design with dark/light mode toggle
- **No authentication required** - perfect for home lab environments
- **Docker deployment** for easy setup and isolation

## Quick Start with Docker Compose

The easiest way to get rpi-mon running on your Raspberry Pi:

### Option 1: Using Pre-built Image (Recommended)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/spindev/rpi-mon.git
   cd rpi-mon
   ```

2. **Start with pre-built image:**
   ```bash
   docker-compose -f docker-compose.prebuilt.yml up -d
   ```

### Option 2: Build from Source

1. **Clone the repository:**
   ```bash
   git clone https://github.com/spindev/rpi-mon.git
   cd rpi-mon
   ```

2. **Start with Docker Compose:**
   ```bash
   docker-compose up -d
   ```

3. **Access the dashboard:**
   Open your browser and go to `http://your-rpi-ip:5000`

That's it! The application will automatically detect your Raspberry Pi hardware and start displaying live metrics.

## Using Pre-built Docker Image

You can use the pre-built image from GitHub Container Registry:

```bash
docker run -d \
  --name rpi-mon \
  -p 5000:5000 \
  -v /proc:/host/proc:ro \
  -v /sys:/host/sys:ro \
  -v /etc:/host/etc:ro \
  --privileged \
  ghcr.io/spindev/rpi-mon:latest
```

## Manual Docker Deployment

If you prefer to build the image yourself:

```bash
docker build -t rpi-mon .

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

- **Raspberry Pi** (any model: Pi 3, Pi 4, Pi 5, etc.)
- **Docker and Docker Compose** installed
- **Network access** to reach the web interface

## Configuration

The application works out of the box with no configuration needed. It automatically:
- Detects your Raspberry Pi model
- Monitors system resources
- Provides real-time updates
- Adapts the interface for mobile devices

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.