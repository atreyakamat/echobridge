# EchoBridge - Build & Run Instructions

## Quick Start

### Option 1: Using Visual Studio 2022

1. Open `EchoBridge.csproj` in Visual Studio 2022
2. Restore NuGet packages (automatic on build)
3. Press F5 to build and run

### Option 2: Using Command Line

```powershell
# Navigate to project directory
cd c:\Projects\echobridge

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Run the application
dotnet run --configuration Release
```

## Initial Setup

### 1. Configure Spotify (Optional)

If you want to use the Spotify widget:

1. Create a Spotify Developer account: https://developer.spotify.com/dashboard
2. Create a new app
3. Copy `spotify_config.json.template` to `spotify_config.json`
4. Fill in your Client ID and Client Secret:

```json
{
  "client_id": "your_spotify_client_id",
  "client_secret": "your_spotify_client_secret",
  "redirect_uri": "http://localhost:5000/callback"
}
```

### 2. Test Audio Devices

1. Launch EchoBridge
2. The app will automatically detect your audio output devices
3. Click "🔄 Refresh Device List" if devices don't appear

## Usage

### Basic Audio Routing

1. **Add Output Devices**
   - Click on device names in the "Add Output Device" section
   - You can add up to 5 devices

2. **Start Routing**
   - Click "▶ Start Routing" button
   - System audio will now play through all selected devices

3. **Adjust Settings**
   - **Volume**: Control each device's volume independently
   - **Delay**: Add delay compensation for Bluetooth devices
   - **Effects**: Toggle FX like EQ, Reverb, Bass Boost, etc.

### Effect Controls

Each device has independent effects:

- **EQ**: 3-band equalizer
- **Bass Boost**: Low-frequency enhancement
- **Reverb**: Spatial reverb effect
- **Delay**: Echo/delay effect
- **Compressor**: Dynamic range compression
- **Limiter**: Prevent audio clipping

### Spotify Widget

1. Click "Connect" in the Spotify section (after configuration)
2. View now-playing track info
3. Use play/pause and skip controls
4. Album art will display when available

## Troubleshooting

### No Audio Output

- Ensure at least one device is added
- Check Windows audio settings
- Try clicking "🔄 Refresh Device List"
- Verify device is not in use by another application

### Bluetooth Latency

- Use the Delay slider (typically 100-200ms for Bluetooth)
- Fine-tune until audio is synchronized

### Spotify Not Connecting

- Verify credentials in `spotify_config.json`
- Check internet connection
- Ensure redirect URI matches in Spotify Developer Dashboard
- Note: Client Credentials flow has limited player access

### High CPU Usage

- Reduce number of active effects
- Lower buffer sizes in audio settings
- Close other audio applications

## Performance Tips

- Start with fewer devices and add more as needed
- Disable unused effects to reduce CPU load
- Use wired connections when possible for lowest latency
- Keep Windows audio drivers updated

## System Requirements

- **OS**: Windows 10/11 (64-bit)
- **CPU**: Dual-core processor or better
- **RAM**: 4GB minimum, 8GB recommended
- **.NET**: .NET 8 Runtime (included with SDK)
- **Audio**: WASAPI-compatible audio devices

## Advanced Configuration

### Custom FX Chains

You can modify effect parameters by editing the FX classes in:
- `Audio/Effects/Equalizer.cs`
- `Audio/Effects/Reverb.cs`
- `Audio/Effects/BassBoost.cs`
- etc.

### Buffer Sizes

Default buffer size is 2 seconds. To adjust, modify in `OutputDevice.cs`:

```csharp
BufferLength = format.AverageBytesPerSecond * 2 // 2 seconds
```

## Building for Distribution

```powershell
# Publish self-contained executable
dotnet publish --configuration Release --runtime win-x64 --self-contained true /p:PublishSingleFile=true

# Output will be in: bin/Release/net8.0-windows/win-x64/publish/
```

## Getting Help

- Check the README.md for detailed documentation
- Review inline code comments
- Submit issues on GitHub (if applicable)

---

**Enjoy using EchoBridge! 🎧**
