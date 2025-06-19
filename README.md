# MiniBrowser - Quick Website Launcher & Auto Translator

A lightweight Windows application that allows you to quickly open any website using a configurable keyboard shortcut, or instantly translate clipboard text using Yandex Translate.

## Features

- **Global Hotkey**: Works system-wide, even when the application isn't focused
- **Configurable**: Change both the website URL and keyboard shortcut
- **System Tray**: Runs quietly in the background
- **Single Instance**: Only one copy runs at a time
- **Default Shortcut**: Ctrl+Q (configurable)
- **App Mode**: Opens websites in minimal browser windows without toolbars (Chrome/Edge)
- **Clean UI**: No address bar, bookmarks, or browser chrome - just the website content
- **Auto Translation**: Instant clipboard text translation with configurable languages
- **Dual Mode**: Switch between website launcher and translator modes

## Building the Application

### Prerequisites
- .NET 6.0 SDK or later
- Windows 10/11

### Build Instructions

1. Open a command prompt in the project directory
2. Run the following command:
```bash
dotnet build -c Release
```

3. The executable will be created in `bin/Release/net6.0-windows/`

### Creating a Standalone Executable

To create a self-contained executable that doesn't require .NET to be installed:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The standalone executable will be in `bin/Release/net6.0-windows/win-x64/publish/`

## Usage

1. **Starting the Application**: Double-click `MiniBrowser.exe`
   - The application will start minimized in the system tray
   - Look for the blue "B" icon in your system tray

2. **Using the Hotkey**: 
   - Press Ctrl+Q (default) to open the configured website
   - The hotkey works globally, even when other applications are focused

3. **Configuring Settings**:
   - Right-click the system tray icon
   - Select "Settings"
   - Change the website URL and/or keyboard shortcut
   - Click "OK" to save

4. **Manual Launch**:
   - Right-click the system tray icon
   - Select "Open Website"

5. **Exiting**:
   - Right-click the system tray icon
   - Select "Exit"

## Configuration

Settings are automatically saved to:
`%APPDATA%/MiniBrowser/settings.json`

The configuration includes:
- `WebsiteUrl`: The website to open (default: https://www.google.com)
- `HotkeyModifiers`: Modifier keys (Ctrl, Alt, Shift, Win)
- `HotkeyKey`: The main key to press

## Examples

- **Ctrl+Q**: Open Google (default)
- **Alt+G**: Open Gmail
- **Ctrl+Shift+Y**: Open YouTube
- **Win+B**: Open any bookmarked site

## Troubleshooting

**Hotkey not working?**
- The hotkey combination might be already used by another application
- Try a different combination in Settings
- Make sure at least one modifier key (Ctrl, Alt, Shift, Win) is selected

**Application not starting?**
- Check if it's already running in the system tray
- Only one instance can run at a time

**Website not opening?**
- Make sure the URL is valid
- Use the "Test URL" button in Settings to verify
- URLs without http:// or https:// will automatically get https:// added

## License

This project is provided as-is for educational and personal use purposes. 