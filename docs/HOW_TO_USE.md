# MiniTranslate - Complete User Guide

A lightweight utility for instant, hotkey-based text translation with multiple translation service options and context-aware translations.

## Table of Contents
1. [Download & Installation](#download--installation)
2. [Quick Start](#quick-start)
3. [Translation Services](#translation-services)
4. [Context-Aware Translations](#context-aware-translations)
5. [Configuration](#configuration)
6. [Troubleshooting](#troubleshooting)
7. [Uninstalling](#uninstalling)

## Download & Installation

1. **Download**: Get the `MiniTranslateSetup.exe` installer from the [Releases](../../releases) page
2. **Install**: Run the installer and follow the setup wizard
3. **Launch**: The application will start automatically after installation

## Quick Start

1. **Start the App**: Double-click `MiniTranslate.exe` - a black and yellow icon will appear in your system tray (bottom-right corner)
2. **Copy Text**: Highlight text in any application (browser, PDF, code editor, etc.) and press `Ctrl+C`
3. **Press Hotkey**: Press the global hotkey (default is `Ctrl+Q`, or use `Ctrl+C+C` like DeepL)
4. **Get Translation**: A translation window will appear with your translated text

## Translation Services

MiniTranslate supports four different translation services. Each has its own advantages:

### 1. Translation Server (Recommended)
**Best for**: High-quality translations, works worldwide, secure API key storage, context-aware translations

**Features**:
- Uses ChatGPT API for high-quality translations
- Server-side API key storage (more secure)
- Works in all regions (including Russia where ChatGPT is blocked)
- Native Windows dialog interface
- Supports 35+ languages
- Context-aware translations with requirements input

**Setup Instructions**:
1. **Contact for Token**: Email `senliksefa@gmail.com` to receive your server token
2. **Configure App**: 
   - Right-click the tray icon → Settings
   - Select "Translation Server" as preferred translator
   - Enter server URL: `https://api.sefa.name.tr`
   - Enter your token in the "Translation Server Token" field
3. **Test**: Copy some text and press `Ctrl+Q` to test

**Cost**: Free (server token provided by request)

### 2. ChatGPT Translator
**Best for**: High-quality translations, direct API control, context-aware translations

**Features**:
- Uses ChatGPT API for high-quality translations
- Native Windows dialog interface
- Direct API key management
- Supports 35+ languages
- Context-aware translations with requirements input

**Setup Instructions**:
1. **Get API Key**: 
   - Go to [OpenAI Platform](https://platform.openai.com/api-keys)
   - Sign in or create an account
   - Click "Create new secret key"
   - Copy the API key (starts with `sk-`)
2. **Configure App**:
   - Right-click the tray icon → Settings
   - Select "ChatGPT Translator" as preferred translator
   - Enter your API key in the "ChatGPT API Key" field
3. **Test**: Copy some text and press `Ctrl+Q` to test

**Cost**: Pay-per-use (typically a few cents per translation)

**Note**: Does NOT work in Russia due to ChatGPT being blocked

### 3. Google Translate
**Best for**: Free translations, wide language support

**Features**:
- Free to use
- Supports 100+ languages
- Reliable and fast
- No API key required

**Setup Instructions**:
1. **Configure App**:
   - Right-click the tray icon → Settings
   - Select "Google Translate" as preferred translator
2. **Test**: Copy some text and press `Ctrl+Q` to test

**Cost**: Free

### 4. Yandex Translate
**Best for**: Russian language translations, free service

**Features**:
- Free to use
- Excellent for Russian translations
- Supports 90+ languages
- No API key required

**Setup Instructions**:
1. **Configure App**:
   - Right-click the tray icon → Settings
   - Select "Yandex Translate" as preferred translator
2. **Test**: Copy some text and press `Ctrl+Q` to test

**Cost**: Free

## Context-Aware Translations

Both ChatGPT Translator and Translation Server support context input for more accurate translations. This feature allows you to provide additional information about your translation requirements.

### How to Use Context

1. **Open Translation Interface**: Press your hotkey after copying text
2. **Find Context Field**: Look for the context input field at the bottom of the translation window
3. **Enter Requirements**: Type your translation requirements, such as:
   - "female speaker, formal tone"
   - "technical document, use industry terminology"
   - "casual conversation, informal style"
   - "medical context, use proper medical terms"
   - "translate 'hello' as 'hi' in this context"

### Context Examples

**Speaker Context**:
- "male speaker, formal tone"
- "female speaker, casual conversation"
- "child speaking to adult"

**Domain Context**:
- "technical documentation"
- "medical terminology"
- "legal document"
- "academic paper"
- "creative writing"

**Style Requirements**:
- "formal business communication"
- "informal friendly tone"
- "academic writing style"
- "creative and engaging"

**Specific Terms**:
- "use 'hi' instead of 'hello'"
- "translate 'car' as 'automobile' in this context"
- "prefer British English spelling"

### Benefits of Context

- **More Accurate Translations**: Context helps the AI understand your specific needs
- **Consistent Terminology**: Ensures consistent use of preferred terms
- **Appropriate Tone**: Matches the intended communication style
- **Domain-Specific Language**: Uses appropriate terminology for the subject matter

## Configuration

### Quick Settings (Tray Menu)
Right-click the tray icon for fast access to common settings:
- **Translator**: Switch between translation services
- **Browser**: Choose Chrome, Edge, or Default browser
- **Source Language**: Set the language you're translating from
- **Target Language**: Set the language you want to translate to

### Main Settings Window
Right-click the tray icon and select "Settings" for full configuration:

#### General Settings
- **Global Hotkey**: Configure the keyboard shortcut (default: Ctrl+Q, also supports Ctrl+C+C like DeepL)
- **Window Size**: Set translation window dimensions (default: 850x600)
- **Run at Windows startup**: Auto-start with Windows

#### Translation Settings
- **Preferred Translator**: Choose your translation service
- **Source Language**: Language to translate from (default: English)
- **Target Language**: Language to translate to (default: Russian)
- **Auto Switch Languages**: Automatically detect and switch languages

#### Service-Specific Settings
- **ChatGPT API Key**: Your OpenAI API key (for ChatGPT Translator)
- **Translation Server URL**: Server URL (default: https://api.sefa.name.tr)
- **Translation Server Token**: Your server token (contact senliksefa@gmail.com)

### Popular Language Combinations
- **English → Russian** (en → ru)
- **English → Spanish** (en → es)
- **Russian → English** (ru → en)
- **Chinese → English** (zh → en)
- **French → English** (fr → en)
- **German → English** (de → en)
- **Japanese → English** (ja → en)

### Language Support

MiniTranslate supports **35+ languages** including:

**European Languages**: English, Russian, Spanish, French, German, Italian, Portuguese, Polish, Dutch, Swedish, Danish, Norwegian, Finnish, Czech, Slovak, Slovenian, Estonian, Serbian, Lithuanian, Hungarian, Romanian, Croatian, Greek

**Asian Languages**: Chinese, Japanese, Korean, Arabic, Hindi, Thai, Vietnamese, Indonesian, Malay

**Other Scripts**: Hebrew

### Intelligent Language Detection

The app automatically detects text scripts and suggests optimal language pairs:
- **Cyrillic**: Russian, Ukrainian, Bulgarian, Serbian
- **Latin**: English, European languages
- **Arabic**: Arabic languages
- **Chinese**: Chinese (Simplified/Traditional)
- **Japanese**: Japanese (Hiragana, Katakana, Kanji)
- **Korean**: Korean (Hangul)
- **Greek**: Greek
- **Hebrew**: Hebrew
- **Thai**: Thai
- **Devanagari**: Hindi

## Troubleshooting

### Hotkey Not Working
- Another application might be using the same key combination
- Try choosing a different hotkey in Settings
- Make sure at least one modifier key (Ctrl, Alt, Shift, Win) is selected

### Can't Find the Tray Icon
- Look in the system tray overflow area (click the small `^` arrow in bottom-right)
- Check if the app is running in Task Manager

### Translation Not Working
- Make sure you have copied text to clipboard before pressing hotkey
- Use the "Test Translation" button in Settings to verify configuration
- Check that both source and target languages are selected
- Verify your API key/token is correct (for ChatGPT/Translation Server)

### ChatGPT Translator Issues
- Check that your API key is correct and starts with `sk-`
- Verify you have sufficient credits in your OpenAI account
- Check that the API key is properly saved in Settings
- Make sure your internet connection is working

### Translation Server Issues
- Verify your server token is correct
- Check that the server URL is accessible
- Contact senliksefa@gmail.com if you need a new token
- Ensure your internet connection is working

### Context Not Working
- Context feature is only available with ChatGPT Translator and Translation Server
- Make sure you're using one of these services
- Check that the context field is visible in the translation window
- Try refreshing the translation window if context field doesn't appear

### Application Won't Start
- Check your system tray - only one instance can run at a time
- Try running as administrator if you encounter permission issues
- Check Windows Event Viewer for error messages

## Uninstalling

### Method 1: Using Uninstaller
1. Go to Control Panel → Programs and Features
2. Find "MiniTranslate" and click "Uninstall"
3. Follow the uninstall wizard

### Method 2: Manual Removal
1. **Close the app**: Right-click the tray icon and select "Exit"
2. **Delete files**: Remove the folder where the application is installed
3. **Remove settings**: Delete `%APPDATA%\MiniTranslate` folder to remove saved settings
4. **Remove startup**: If enabled, uncheck "Run at Windows startup" before exiting

## Features

✅ **Global Hotkeys** - Works from any application
✅ **Multiple Translation Services** - Choose from 4 different options
✅ **Context-Aware Translations** - Provide context for more accurate results
✅ **System Tray** - Runs quietly in background
✅ **Single Instance** - Won't create duplicates
✅ **Settings Persistence** - Remembers your configuration
✅ **Clean Interface** - No toolbars or browser UI clutter
✅ **Multi-Language Support** - 35-100+ languages depending on service
✅ **Auto Language Detection** - Automatically detects source language
✅ **Intelligent Script Detection** - Detects Cyrillic, Latin, Arabic, Chinese, Japanese, Korean, Greek, Hebrew, Thai, Devanagari
✅ **Customizable Hotkeys** - Set your preferred keyboard shortcuts
✅ **Startup Integration** - Auto-start with Windows

## Technical Details

- **Framework**: .NET 6.0 Windows Forms
- **Size**: ~139MB (self-contained executable)
- **Requirements**: Windows 10/11 (no .NET installation required)
- **Memory Usage**: ~50MB RAM when running
- **Settings Location**: `%APPDATA%\MiniTranslate\settings.json`
- **Node.js**: Included executable for local web server functionality

## Support

### Getting Help
1. **Check this guide** for common issues and solutions
2. **Test your settings** using the "Test Translation" button
3. **Reset to defaults** by deleting the settings file at `%APPDATA%\MiniTranslate\settings.json`

### Translation Server Support
- **Email**: senliksefa@gmail.com for server token requests
- **Server Status**: Check if the service is available
- **Token Issues**: Contact for new tokens or token renewal

### ChatGPT API Support
- **API Issues**: Check [OpenAI Platform](https://platform.openai.com/usage) for usage and billing
- **Key Management**: Manage your API keys at [OpenAI Platform](https://platform.openai.com/api-keys)

---

**Need More Help?** Check the other documentation files in the `/docs` folder for detailed setup guides for specific translation services. 