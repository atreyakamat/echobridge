# 🚀 Running EchoBridge as Standalone .EXE

## ✅ Executable Created!

Your standalone executable is ready at:

```
📁 c:\Projects\echobridge\bin\Release\net8.0-windows\win-x64\publish\EchoBridge.exe
```

**Size**: ~160 MB (includes all dependencies)

---

## 🎯 How to Run

### Option 1: Double-Click
1. Navigate to: `c:\Projects\echobridge\bin\Release\net8.0-windows\win-x64\publish\`
2. Double-click **EchoBridge.exe**
3. The application will launch!

### Option 2: Command Line
```powershell
cd c:\Projects\echobridge\bin\Release\net8.0-windows\win-x64\publish
.\EchoBridge.exe
```

### Option 3: Create Desktop Shortcut
1. Right-click **EchoBridge.exe**
2. Select "Send to" → "Desktop (create shortcut)"
3. Double-click the desktop icon to launch

---

## 📦 Distribution

### To Share with Others:

**Option A: Share the Folder**
- Copy the entire `publish` folder to another computer
- No .NET installation required!
- Just run EchoBridge.exe

**Option B: Create a ZIP**
```powershell
# From project root
Compress-Archive -Path "bin\Release\net8.0-windows\win-x64\publish\*" -DestinationPath "EchoBridge-v1.0.zip"
```

Then share `EchoBridge-v1.0.zip` - recipients just need to:
1. Extract the ZIP
2. Run EchoBridge.exe

---

## 🎵 Optional: Spotify Configuration

If you want Spotify integration, copy the config file to the publish folder:

```powershell
# Copy the template to publish folder
copy spotify_config.json.template bin\Release\net8.0-windows\win-x64\publish\spotify_config.json

# Then edit the file with your Spotify credentials
```

---

## ⚙️ System Requirements

- **OS**: Windows 10/11 (64-bit)
- **RAM**: 4GB minimum
- **Disk Space**: ~200MB
- **Audio**: WASAPI-compatible audio devices
- **.NET**: NOT required (self-contained!)

---

## 🔧 Rebuilding the .EXE

If you make code changes and want to rebuild:

```powershell
# From project root
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Output will be in: `bin\Release\net8.0-windows\win-x64\publish\`

---

## 📍 Quick Launch Guide

```
1. Navigate to: bin\Release\net8.0-windows\win-x64\publish\
2. Double-click: EchoBridge.exe
3. Add your audio devices
4. Click "▶ Start Routing"
5. Enjoy! 🎧
```

---

## 🎉 You're All Set!

Your standalone EchoBridge application is ready to run on any Windows 10/11 PC without installing .NET!

**Location**: `bin\Release\net8.0-windows\win-x64\publish\EchoBridge.exe`
