# 🎧 EchoBridge

> **Professional Windows Audio Routing & FX Mixer**

EchoBridge is a next-generation audio routing and effects application for Windows, inspired by Voicemeeter but rebuilt with modern technologies and creative flexibility. Route system audio to multiple output devices simultaneously with independent FX processing per device.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)

---

## ✨ Features

- 🔊 **Multi-Device Output**: Route audio to up to 5 devices simultaneously (wired, Bluetooth, HDMI)
- 🎚️ **Independent FX Chains**: Apply custom effects per device (EQ, Reverb, BassBoost, Delay, Compressor, Limiter)
- ⚡ **Real-Time Processing**: Low-latency audio routing with WASAPI loopback
- 🎵 **Spotify Integration**: Now-playing widget with playback controls
- 🌑 **Dark Modern UI**: Sleek, professional interface with glassmorphism design
- 🎯 **Precise Control**: Per-device volume and delay compensation
- 🔄 **Sync Manager**: Automatic buffer alignment for synchronized playback

---

## 🚀 Getting Started

### Prerequisites

- **Windows 10/11** (64-bit)
- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** or **Rider** (optional, for development)
- **Spotify Developer Account** (for widget integration)

### Installation

1. **Clone the repository**
   ```powershell
   git clone https://github.com/yourusername/echobridge.git
   cd echobridge
   ```

2. **Restore NuGet packages**
   ```powershell
   dotnet restore
   ```

3. **Build the project**
   ```powershell
   dotnet build
   ```

4. **Run EchoBridge**
   ```powershell
   dotnet run
   ```

---

## 🎵 Spotify Setup (Optional)

To enable the Spotify widget:

1. Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Create a new app
3. Note your **Client ID** and **Client Secret**
4. Add redirect URI: `http://localhost:5000/callback`
5. Copy `spotify_config.json.template` to `spotify_config.json`
6. Fill in your credentials:
   ```json
   {
     "client_id": "your_client_id_here",
     "client_secret": "your_client_secret_here",
     "redirect_uri": "http://localhost:5000/callback"
   }
   ```

---

## 📖 Usage Guide

### 1. Adding Output Devices

1. Launch EchoBridge
2. Click on any available device in the "Add Output Device" section
3. The device will appear in the output devices list
4. Configure volume, delay, and effects for each device

### 2. Applying Effects

Each device has its own FX chain with toggleable effects:

- **EQ**: 3-band equalizer (Low/Mid/High)
- **Bass Boost**: Low-frequency enhancement
- **Reverb**: Spatial reverb with adjustable room size
- **Delay**: Echo effect with feedback control
- **Compressor**: Dynamic range compression
- **Limiter**: Output protection

Toggle effects by clicking the FX buttons on each device panel.

### 3. Starting Audio Routing

1. Add at least one output device
2. Click **"▶ Start Routing"**
3. System audio will now be routed to all selected devices
4. Adjust volume and effects in real-time

### 4. Sync and Delay Compensation

If devices have latency issues (especially Bluetooth):

- Use the **Delay (ms)** slider to compensate
- Typical Bluetooth delay: 100-200ms
- Fine-tune until all outputs are synchronized

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│           WASAPI Loopback Capture           │
│         (System Audio Input)                │
└──────────────────┬──────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────┐
│            Audio Router                     │
│    (Distributes to all devices)             │
└──────┬──────────┬──────────┬────────────────┘
       │          │          │
       ▼          ▼          ▼
   ┌────────┐ ┌────────┐ ┌────────┐
   │Device 1│ │Device 2│ │Device 3│
   │        │ │        │ │        │
   │ FX     │ │ FX     │ │ FX     │
   │ Chain  │ │ Chain  │ │ Chain  │
   └────────┘ └────────┘ └────────┘
```

### Core Components

- **LoopbackCapture**: Captures system audio via WASAPI
- **AudioRouter**: Manages device registration and stream distribution
- **OutputDevice**: Individual device with buffer, FX chain, and controls
- **FxChain**: DSP effects pipeline
- **Effects**: Modular audio processors (EQ, Reverb, etc.)

---

## 🎨 Tech Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 8 (WPF) |
| Audio Engine | NAudio |
| UI Library | ModernWpfUI |
| MVVM | CommunityToolkit.Mvvm |
| API Integration | SpotifyAPI.Web |
| Serialization | Newtonsoft.Json |

---

## 🛠️ Development

### Project Structure

```
EchoBridge/
├── Audio/
│   ├── AudioRouter.cs           # Main routing engine
│   ├── OutputDevice.cs          # Device management
│   ├── FxChain.cs               # Effect chain processor
│   ├── LoopbackCapture.cs       # System audio capture
│   └── Effects/
│       ├── Equalizer.cs         # 3-band EQ
│       ├── Reverb.cs            # Reverb effect
│       ├── BassBoost.cs         # Bass enhancement
│       ├── Delay.cs             # Echo/delay
│       ├── Compressor.cs        # Dynamic compression
│       └── Limiter.cs           # Output limiter
├── UI/
│   ├── MainWindow.xaml          # Main application window
│   ├── MainWindowViewModel.cs   # Main view model
│   ├── SpotifyWidgetViewModel.cs# Spotify integration
│   ├── Styles.xaml              # Dark theme styles
│   └── Converters.cs            # Value converters
├── Services/
│   ├── SpotifyService.cs        # Spotify API wrapper
│   └── SpotifyConfig.cs         # Configuration management
├── App.xaml                     # Application entry
└── EchoBridge.csproj            # Project file
```

### Building from Source

```powershell
# Clone repository
git clone https://github.com/yourusername/echobridge.git
cd echobridge

# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Release

# Run
dotnet run --configuration Release
```

### Running Tests

```powershell
dotnet test
```

---

## 🎯 Roadmap

- [x] Core audio routing
- [x] Multi-device output
- [x] Real-time FX processing
- [x] Dark themed UI
- [x] Spotify widget (basic)
- [ ] Bluetooth sync optimization
- [ ] FX preset save/load
- [ ] Custom themes/skins
- [ ] Virtual cable (for OBS/Discord)
- [ ] Equalizer visualization
- [ ] Audio spectrum analyzer
- [ ] ASIO support
- [ ] VST plugin support

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Inspired by [Voicemeeter](https://vb-audio.com/Voicemeeter/)
- Built with [NAudio](https://github.com/naudio/NAudio)
- UI styled with [ModernWpfUI](https://github.com/Kinnara/ModernWpf)
- Spotify integration via [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET)

---

## 📧 Contact

For questions, feedback, or support:

- Open an [Issue](https://github.com/yourusername/echobridge/issues)
- Email: your.email@example.com

---

## ⚠️ Disclaimer

EchoBridge is provided "as is" without warranty. Use at your own risk. Always test with low volumes first to protect your hearing and equipment.

---

<div align="center">
  <sub>Built with ❤️ for audio enthusiasts</sub>
</div>
