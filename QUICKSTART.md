# 🚀 EchoBridge - Quick Start Guide

## Run the Application

```powershell
cd c:\Projects\echobridge
dotnet run
```

## First Time Setup (5 steps)

### 1️⃣ Launch EchoBridge
- The app window will open with a dark theme
- You'll see "OUTPUT DEVICES" section (initially empty)

### 2️⃣ Add Your First Device
- Scroll down to "Add Output Device" section
- Click on your desired output device name
- The device appears in the OUTPUT DEVICES list above

### 3️⃣ Configure the Device (Optional)
- **Volume**: Adjust the volume slider (0-100%)
- **Delay**: Add delay compensation if needed (0-500ms)
- **Effects**: Click FX buttons to toggle effects
  - EQ, Bass Boost, Reverb, Delay, Compressor, Limiter

### 4️⃣ Add More Devices (Optional)
- Repeat step 2 to add up to 5 devices total
- Each device can have independent settings

### 5️⃣ Start Audio Routing
- Click the green **"▶ Start Routing"** button at the top
- System audio now plays through ALL selected devices! 🎉

---

## Common Use Cases

### 🎧 Use Case 1: Bluetooth + Wired Speakers
**Goal**: Play audio on both Bluetooth headphones and wired speakers

1. Add "Speakers (Your Speaker Device)"
2. Add "Bluetooth Audio (Your Headphones)"
3. If Bluetooth has delay, add 100-200ms delay compensation
4. Start routing

### 🔊 Use Case 2: Multi-Room Audio
**Goal**: Play music in multiple rooms

1. Add each room's audio device
2. Adjust volume per room
3. Start routing
4. All rooms play synchronized!

### 🎵 Use Case 3: Enhanced Bass for Headphones
**Goal**: Add bass boost and EQ to headphones

1. Add your headphone device
2. Toggle "Bass Boost" effect
3. Toggle "EQ" effect
4. Start routing
5. Enjoy enhanced audio!

### 🎮 Use Case 4: Gaming with Audio Effects
**Goal**: Add reverb and delay to game audio

1. Add your gaming headset
2. Toggle "Reverb" effect (for spatial depth)
3. Optionally add "Delay" for echo
4. Start routing
5. Experience immersive game audio

---

## Troubleshooting

### ❌ No Sound
- **Check**: Is routing started? (Green button should say "⏸ Stop Routing")
- **Check**: Is volume above 0%?
- **Check**: Is the device actually working in Windows?
- **Fix**: Try refreshing device list

### ❌ Bluetooth Delay/Echo
- **Problem**: Audio from Bluetooth is slightly behind wired
- **Fix**: Increase Delay slider on Bluetooth device (try 150ms)

### ❌ Device Not Listed
- **Fix**: Click "🔄 Refresh Device List"
- **Check**: Is the device enabled in Windows Sound settings?

### ❌ High CPU Usage
- **Fix**: Disable unused effects
- **Fix**: Reduce number of active devices
- **Fix**: Close other audio applications

---

## Tips & Tricks

💡 **Tip 1**: Start with one device first to test
💡 **Tip 2**: Use Limiter effect to prevent audio clipping
💡 **Tip 3**: Save your favorite device + FX combinations (feature coming soon!)
💡 **Tip 4**: Bluetooth delay is usually 100-200ms
💡 **Tip 5**: Compressor effect is great for music, less ideal for speech

---

## Spotify Widget (Optional)

### Setup
1. Create Spotify Developer account: https://developer.spotify.com/dashboard
2. Copy `spotify_config.json.template` → `spotify_config.json`
3. Fill in your credentials
4. Restart EchoBridge

### Usage
- Widget shows on right sidebar
- Click "Connect" to authenticate
- See now-playing track info
- Use playback controls (play/pause/skip)

**Note**: Full player control requires OAuth2 user authentication (future feature)

---

## Keyboard Shortcuts (Future)
- `Ctrl + Space`: Start/Stop routing
- `Ctrl + R`: Refresh device list
- `Ctrl + S`: Save preset (planned)

---

## Need Help?

📖 **Full Documentation**: See `README.md`  
🔧 **Setup Guide**: See `SETUP.md`  
📊 **Project Details**: See `PROJECT_SUMMARY.md`

---

## Enjoy EchoBridge! 🎧✨

**Made with ❤️ for audio enthusiasts**
