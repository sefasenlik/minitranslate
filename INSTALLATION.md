# MiniBrowser Installation & Usage Guide
## Quick Website Launcher & Auto Translator

## Quick Start

### Option 1: Download Pre-built Executable (Recommended)
1. Go to the [Releases](../../releases) section on GitHub
2. Download the latest `MiniTranslator.exe` from the release assets
3. Double-click `MiniTranslator.exe` to start the application
4. The application will start minimized in the system tray (look for a teal/white icon)
5. Press **Ctrl+Q** to activate translation mode (default)

### Option 2: Build from Source
1. Clone this repository
2. Open the project in Visual Studio or use .NET CLI
3. Build the project: `dotnet publish -c Release -r win-x64 --self-contained`
4. Navigate to `bin/Release/net6.0-windows/win-x64/publish/`
5. Run `MiniTranslator.exe`

### Option 3: Use the Batch File
1. Double-click `run-minitranslator.bat` in the project root (if available)
2. This will start the application and show helpful instructions

## First Time Setup

### Translation Mode (Default & Recommended)
1. Look for the blue "T" icon in your system tray (bottom-right corner)
2. Right-click the icon and select "Settings"
3. **Translation Mode is enabled by default** - auto-translation functionality is ready
4. Select your **Source Language** (e.g., English) and **Target Language** (e.g., Русский)
5. Configure your keyboard shortcut (default: Ctrl+Q)
6. Click "Test Translation" - copy some text first, then test
7. Click "OK" to save settings

### Alternative: Custom Website Mode
- **Uncheck "Translation Mode"** to switch to custom website launching
- Enter your preferred website URL (e.g., `https://gmail.com`, `https://youtube.com`)
- Click "Test Custom Website" to verify your URL works
- Configure hotkey and save

## Using the Application

### Translation Mode (Default)
1. **Copy text** you want to translate (Ctrl+C)
2. **Press your hotkey** (default: Ctrl+Q) from anywhere in Windows
3. **Yandex Translate opens** in app mode with your text pre-loaded
4. **Translation appears** instantly in your configured target language

### Custom Website Mode
- **Global Hotkey**: Press your configured shortcut anywhere in Windows  
- **Manual**: Right-click the tray icon and select "Open/Translate"
- **Double-click**: Double-click the tray icon

### Managing the Application
- **Settings**: Right-click tray icon → "Settings"
- **Exit**: Right-click tray icon → "Exit"

### Auto-Start with Windows (Optional)
To have MiniTranslator start automatically when Windows boots:

1. Press `Win+R`, type `shell:startup`, and press Enter
2. Copy `MiniTranslator.exe` from the `publish/` folder to the Startup folder
3. The application will now start with Windows

## Features

✅ **Global Hotkeys** - Works from any application
✅ **Customizable** - Change URL and keyboard shortcuts
✅ **System Tray** - Runs quietly in background
✅ **Single Instance** - Won't create duplicates
✅ **URL Validation** - Automatically adds HTTPS if needed
✅ **Settings Persistence** - Remembers your configuration
✅ **App Mode** - Opens websites in minimal browser windows (Chrome/Edge)
✅ **Clean Interface** - No toolbars, address bars, or browser UI clutter
✅ **Auto Translation** - Instant clipboard text translation using Yandex Translate
✅ **Dual Mode** - Switch between website launcher and translator functionality
✅ **Multi-Language** - Support for 70+ languages including English, Russian, Spanish, etc.

## Troubleshooting

### Hotkey Not Working?
- Another application might be using the same combination
- Try a different key combination in Settings
- Make sure at least one modifier key is selected

### Can't Find the Tray Icon?
- Look in the system tray (bottom-right corner)
- Click the small arrow (^) to show hidden icons
- The icon is a blue square with a white "B"

### Website Won't Open?
- Check that the URL is valid using the "Test URL" button
- Make sure you have an internet connection
- Try adding `https://` to the beginning of the URL

### Browser Compatibility
- **Best Experience**: Google Chrome or Microsoft Edge installed
- **App Mode**: Websites open in clean, minimal windows without toolbars
- **Fallback**: Uses default browser if Chrome/Edge not available
- **Auto-Detection**: Automatically finds and uses Chrome or Edge

### Translation Not Working?
- Make sure you have **copied text to clipboard** before pressing the hotkey
- **Check language settings** - both source and target languages must be selected
- **Enable Translation Mode** in settings if not already enabled
- Try the **"Test Translation"** button in settings with some copied text

### Application Won't Start?
- Check if it's already running in the system tray
- Only one instance can run at a time
- Try running as administrator if needed

## Configuration File

Settings are stored in: `%APPDATA%\MiniBrowser\settings.json`

Example configurations:

**Translation Mode:**
```json
{
  "WebsiteUrl": "https://www.google.com",
  "HotkeyModifiers": 2,
  "HotkeyKey": 81,
  "SourceLanguage": "en",
  "TargetLanguage": "ru",
  "UseTranslateMode": true
}
```

**Website Mode:**
```json
{
  "WebsiteUrl": "https://gmail.com",
  "HotkeyModifiers": 1,
  "HotkeyKey": 71,
  "UseTranslateMode": false
}
```

## Technical Details

- **Framework**: .NET 6.0 Windows Forms
- **Size**: ~139MB (self-contained executable)
- **Requirements**: Windows 10/11 (no .NET installation required)
- **Memory Usage**: ~50MB RAM when running

## Popular Hotkey Combinations

### Translation Mode (Default)
- **Ctrl+Q**: Quick translate (default)
- **Alt+T**: Translate text
- **Ctrl+Shift+T**: Translation hotkey
- **Win+T**: Windows + Translate

### Custom Website Mode  
- **Alt+G**: Gmail
- **Ctrl+Shift+Y**: YouTube  
- **Win+B**: Bookmarks/favorite site
- **Alt+N**: News site
- **Ctrl+Alt+M**: Email

## Popular Language Combinations

- **English → Russian** (en → ru)
- **English → Spanish** (en → es)  
- **Russian → English** (ru → en)
- **Chinese → English** (zh → en)
- **French → English** (fr → en)
- **German → English** (de → en)
- **Japanese → English** (ja → en)

## Uninstalling

1. Right-click the tray icon and select "Exit"
2. Delete the MiniTranslator folder
3. Optionally delete settings: `%APPDATA%\MiniBrowser\`

---

**Need Help?** Check the README.md file for more technical information. 