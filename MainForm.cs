using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MiniTranslate
{
    public static class LanguageDetector
    {
        // Script detection based on Unicode ranges
        public static string DetectPrimaryScript(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "unknown";

            var scriptCounts = new Dictionary<string, int>
            {
                {"cyrillic", 0},
                {"latin", 0},
                {"arabic", 0},
                {"chinese", 0},
                {"japanese", 0},
                {"korean", 0},
                {"greek", 0},
                {"hebrew", 0},
                {"thai", 0},
                {"devanagari", 0}
            };

            foreach (char c in text)
            {
                // Skip spaces, punctuation, and numbers
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsDigit(c) || char.IsSymbol(c))
                    continue;

                int code = (int)c;

                // Cyrillic (Russian, Ukrainian, Bulgarian, etc.)
                if ((code >= 0x0400 && code <= 0x04FF) || (code >= 0x0500 && code <= 0x052F))
                    scriptCounts["cyrillic"]++;
                // Latin (English, European languages)
                else if ((code >= 0x0041 && code <= 0x005A) || (code >= 0x0061 && code <= 0x007A) ||
                         (code >= 0x00C0 && code <= 0x024F))
                    scriptCounts["latin"]++;
                // Arabic
                else if ((code >= 0x0600 && code <= 0x06FF) || (code >= 0x0750 && code <= 0x077F))
                    scriptCounts["arabic"]++;
                // Chinese (CJK Unified Ideographs)
                else if ((code >= 0x4E00 && code <= 0x9FFF) || (code >= 0x3400 && code <= 0x4DBF))
                    scriptCounts["chinese"]++;
                // Japanese (Hiragana, Katakana)
                else if ((code >= 0x3040 && code <= 0x309F) || (code >= 0x30A0 && code <= 0x30FF))
                    scriptCounts["japanese"]++;
                // Korean (Hangul)
                else if ((code >= 0xAC00 && code <= 0xD7AF) || (code >= 0x1100 && code <= 0x11FF))
                    scriptCounts["korean"]++;
                // Greek
                else if ((code >= 0x0370 && code <= 0x03FF) || (code >= 0x1F00 && code <= 0x1FFF))
                    scriptCounts["greek"]++;
                // Hebrew
                else if (code >= 0x0590 && code <= 0x05FF)
                    scriptCounts["hebrew"]++;
                // Thai
                else if (code >= 0x0E00 && code <= 0x0E7F)
                    scriptCounts["thai"]++;
                // Devanagari (Hindi)
                else if (code >= 0x0900 && code <= 0x097F)
                    scriptCounts["devanagari"]++;
            }

            // Find the most common script
            var maxScript = scriptCounts.Aggregate((x, y) => x.Value > y.Value ? x : y);
            return maxScript.Value > 0 ? maxScript.Key : "latin"; // Default to latin if no script detected
        }

        public static string GetSuggestedSourceLanguage(string detectedScript, string currentSourceLang, string currentTargetLang)
        {
            var scriptToLanguageMap = new Dictionary<string, string[]>
            {
                {"cyrillic", new[] {"ru", "uk", "bg", "sr"}},
                {"latin", new[] {"en", "es", "fr", "de", "it", "pt", "pl", "nl", "sv", "da", "no", "fi", "cs", "sk", "sl", "et", "sr", "lt", "hu", "ro", "hr", "tr", "vi", "id", "ms"}},
                {"arabic", new[] {"ar"}},
                {"chinese", new[] {"zh"}},
                {"japanese", new[] {"ja"}},
                {"korean", new[] {"ko"}},
                {"greek", new[] {"el"}},
                {"hebrew", new[] {"he"}},
                {"thai", new[] {"th"}},
                {"devanagari", new[] {"hi"}}
            };

            if (!scriptToLanguageMap.ContainsKey(detectedScript))
                return currentSourceLang;

            var possibleLanguages = scriptToLanguageMap[detectedScript];
            
            // If current source language matches the detected script, keep it
            if (possibleLanguages.Contains(currentSourceLang))
                return currentSourceLang;

            // Otherwise, return the first (most common) language for this script
            return possibleLanguages[0];
        }

        public static (string newSource, string newTarget) GetAutoSwitchedLanguages(string text, string currentSource, string currentTarget)
        {
            var detectedScript = DetectPrimaryScript(text);
            var suggestedSource = GetSuggestedSourceLanguage(detectedScript, currentSource, currentTarget);

            // If the suggested source is different from current source, switch
            if (suggestedSource != currentSource)
            {
                // If the suggested source matches current target, swap them
                if (suggestedSource == currentTarget)
                {
                    return (currentTarget, currentSource);
                }
                // Otherwise, use suggested source and keep current target
                else
                {
                    return (suggestedSource, currentTarget);
                }
            }

            // No change needed
            return (currentSource, currentTarget);
        }
    }

    public static class LanguageProvider
    {
        public static Dictionary<string, string> GetLanguages()
        {
            return new Dictionary<string, string>
            {
                {"auto", "Detect language"}, {"en", "English"}, {"ru", "Русский"}, {"es", "Español"}, {"fr", "Français"},
                {"de", "Deutsch"}, {"it", "Italiano"}, {"pt", "Português"}, {"zh", "中文"},
                {"ja", "日本語"}, {"ko", "한국어"}, {"ar", "العربية"}, {"hi", "हिन्दी"},
                {"tr", "Türkçe"}, {"pl", "Polski"}, {"nl", "Nederlands"}, {"sv", "Svenska"},
                {"da", "Dansk"}, {"no", "Norsk"}, {"fi", "Suomi"}, {"cs", "Čeština"},
                {"uk", "Українська"}, {"bg", "Български"}, {"hr", "Hrvatski"}, {"sk", "Slovenčina"},
                {"sl", "Slovenščina"}, {"et", "Eesti"}, {"sr", "Српски"}, {"lt", "Lietuvių"},
                {"hu", "Magyar"}, {"ro", "Română"}, {"el", "Ελληνικά"}, {"he", "עברית"},
                {"th", "ไทย"}, {"vi", "Tiếng Việt"}, {"id", "Indonesia"}, {"ms", "Melayu"}
            };
        }
    }

    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private AppSettings settings;
        private int hotKeyId = 1;
        private static readonly HttpClient httpClient = new HttpClient();
        
        // Double key detection
        private DateTime lastKeyPress = DateTime.MinValue;
        private const int DOUBLE_KEY_TIMEOUT_MS = 300; // 300ms timeout for double key detection

        // Windows API for global hotkeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Hotkey modifiers
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        // Windows message for hotkey
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        private Process chatGptWindowProcess = null;
        private Process translationServerWindowProcess = null;

        // Clipboard monitoring for double copy detection
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        private DateTime lastClipboardCopy = DateTime.MinValue;
        private bool clipboardListenerActive = false;

        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
            InitializeTrayIcon();
            
            // Determine hotkey type based on key combination
            // If CTRL+C is selected, use double press (clipboard monitoring)
            // Otherwise, use single press (global hotkey)
            bool isCtrlC = (settings.HotkeyModifiers & MOD_CONTROL) != 0 && settings.HotkeyKey == (int)Keys.C;
            
            if (isCtrlC)
            {
                StartClipboardListener();
            }
            else
            {
                RegisterGlobalHotKey();
            }
            
            UpdateTrayIconText();
            
            // Hide the form initially
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;
        }

        private void InitializeComponent()
        {
            this.Text = "MiniTranslate";
            this.Size = new Size(1, 1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayIcon = new NotifyIcon()
            {
                Text = "MiniTranslate", // Set initial placeholder text
                Icon = LoadCustomIcon(),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            BuildTrayMenu();
            trayIcon.DoubleClick += OnOpenWebsite;
        }

        private void BuildTrayMenu()
        {
            trayMenu.Items.Clear();

            trayMenu.Items.Add("Open/Translate", null, OnOpenWebsite);
            trayMenu.Items.Add("-");

            // Switch Languages
            var switchLanguagesItem = new ToolStripMenuItem("Switch Languages", null, OnSwitchLanguages);
            switchLanguagesItem.Enabled = settings.SourceLanguage != "auto";
            trayMenu.Items.Add(switchLanguagesItem);
            trayMenu.Items.Add("-");

            // Translator Service Menu
            var translatorMenu = new ToolStripMenuItem("Translator");
            var yandexItem = new ToolStripMenuItem("Yandex Translate", null, OnTranslatorChanged) { Tag = TranslatorType.Yandex };
            var googleItem = new ToolStripMenuItem("Google Translate", null, OnTranslatorChanged) { Tag = TranslatorType.Google };
            var chatgptItem = new ToolStripMenuItem("ChatGPT Translator", null, OnTranslatorChanged) { Tag = TranslatorType.ChatGPT };
            var translationServerItem = new ToolStripMenuItem("Translation Server", null, OnTranslatorChanged) { Tag = TranslatorType.TranslationServer };
            yandexItem.Checked = settings.PreferredTranslator == TranslatorType.Yandex;
            googleItem.Checked = settings.PreferredTranslator == TranslatorType.Google;
            chatgptItem.Checked = settings.PreferredTranslator == TranslatorType.ChatGPT;
            translationServerItem.Checked = settings.PreferredTranslator == TranslatorType.TranslationServer;
            translatorMenu.DropDownItems.Add(yandexItem);
            translatorMenu.DropDownItems.Add(googleItem);
            translatorMenu.DropDownItems.Add(chatgptItem);
            translatorMenu.DropDownItems.Add(translationServerItem);
            trayMenu.Items.Add(translatorMenu);

            // Browser Menu
            var browserMenu = new ToolStripMenuItem("Browser");
            var chromeItem = new ToolStripMenuItem("Chrome", null, OnBrowserChanged) { Tag = BrowserType.Chrome };
            var edgeItem = new ToolStripMenuItem("Edge", null, OnBrowserChanged) { Tag = BrowserType.Edge };
            var defaultItem = new ToolStripMenuItem("Default", null, OnBrowserChanged) { Tag = BrowserType.Default };
            chromeItem.Checked = settings.PreferredBrowser == BrowserType.Chrome;
            edgeItem.Checked = settings.PreferredBrowser == BrowserType.Edge;
            defaultItem.Checked = settings.PreferredBrowser == BrowserType.Default;
            browserMenu.DropDownItems.Add(chromeItem);
            browserMenu.DropDownItems.Add(edgeItem);
            browserMenu.DropDownItems.Add(defaultItem);
            trayMenu.Items.Add(browserMenu);
            
            // Language Menus
            var sourceLangMenu = new ToolStripMenuItem("Source Language");
            var targetLangMenu = new ToolStripMenuItem("Target Language");

            var languages = LanguageProvider.GetLanguages();
            foreach (var lang in languages)
            {
                // Add to Source Language Menu
                var sourceItem = new ToolStripMenuItem(lang.Value, null, OnSourceLanguageChanged) { Tag = lang.Key };
                sourceItem.Checked = settings.SourceLanguage == lang.Key;
                sourceLangMenu.DropDownItems.Add(sourceItem);

                // Add to Target Language Menu (excluding "auto" - Detect language)
                if (lang.Key != "auto")
                {
                    var targetItem = new ToolStripMenuItem(lang.Value, null, OnTargetLanguageChanged) { Tag = lang.Key };
                    targetItem.Checked = settings.TargetLanguage == lang.Key;
                    targetLangMenu.DropDownItems.Add(targetItem);
                }
            }

            trayMenu.Items.Add(sourceLangMenu);
            trayMenu.Items.Add(targetLangMenu);

            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Settings", null, OnSettings);
            trayMenu.Items.Add("Exit", null, OnExit);
        }

        private Icon LoadCustomIcon()
        {
            // Try to load icon from multiple possible locations
            string[] iconPaths = {
                "icon.ico",  // Current directory first
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico"),  // Application directory
                Path.Combine(Environment.CurrentDirectory, "icon.ico"),  // Working directory
                Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "icon.ico")  // Executable directory
            };

            foreach (string iconPath in iconPaths)
            {
                try
                {
                    if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                    {
                        return new Icon(iconPath);
                    }
                }
                catch
                {
                    // Continue to next path if this one fails
                    continue;
                }
            }
            
            // If no icon file found, create default
            return CreateDefaultIcon();
        }

        private Icon CreateDefaultIcon()
        {
            // Create a simple default icon with "T" for Translator
            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.DarkBlue, 0, 0, 16, 16);
                g.FillRectangle(Brushes.White, 2, 2, 12, 12);
                g.DrawString("T", new Font("Arial", 8, FontStyle.Bold), Brushes.DarkBlue, 3, 1);
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void LoadSettings()
        {
            settings = AppSettings.Load();
            // DO NOT update tray icon text here, as the icon may not exist yet.
        }

        private void UpdateTrayIconText()
        {
            if (trayIcon == null) return; // Add a safeguard

            var hotkeyText = GetHotkeyDisplayString();
            string translatorName;
            switch (settings.PreferredTranslator)
            {
                case TranslatorType.Google:
                    translatorName = "Google";
                    break;
                case TranslatorType.ChatGPT:
                    translatorName = "ChatGPT";
                    break;
                case TranslatorType.TranslationServer:
                    translatorName = "Translation Server";
                    break;
                default:
                    translatorName = "Yandex";
                    break;
            }
            trayIcon.Text = $"MiniTranslate - {hotkeyText} to translate clipboard ({settings.SourceLanguage}→{settings.TargetLanguage}) via {translatorName}";
        }

        private string GetHotkeyDisplayString()
        {
            var parts = new List<string>();
            
            if ((settings.HotkeyModifiers & MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((settings.HotkeyModifiers & MOD_ALT) != 0) parts.Add("Alt");
            if ((settings.HotkeyModifiers & MOD_SHIFT) != 0) parts.Add("Shift");
            if ((settings.HotkeyModifiers & MOD_WIN) != 0) parts.Add("Win");
            
            parts.Add(((Keys)settings.HotkeyKey).ToString());
            
            var hotkeyString = string.Join("+", parts);
            
            // Add double press indicator for CTRL+C
            bool isCtrlC = (settings.HotkeyModifiers & MOD_CONTROL) != 0 && settings.HotkeyKey == (int)Keys.C;
            if (isCtrlC)
            {
                hotkeyString += " (Double)";
            }
            
            return hotkeyString;
        }

        private void RegisterGlobalHotKey()
        {
            UnregisterHotKey(this.Handle, hotKeyId);
            
            // Only register global hotkey if not CTRL+C (which uses clipboard monitoring)
            bool isCtrlC = (settings.HotkeyModifiers & MOD_CONTROL) != 0 && settings.HotkeyKey == (int)Keys.C;
            
            if (!isCtrlC)
            {
                if (!RegisterHotKey(this.Handle, hotKeyId, settings.HotkeyModifiers, settings.HotkeyKey))
                {
                    MessageBox.Show($"Failed to register hotkey {GetHotkeyDisplayString()}. It might be already in use.",
                        "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            UpdateTrayIcon();
        }

        protected override void WndProc(ref Message m)
        {
            // Check if current hotkey is CTRL+C (which uses clipboard monitoring)
            bool isCtrlC = (settings.HotkeyModifiers & MOD_CONTROL) != 0 && settings.HotkeyKey == (int)Keys.C;
            
            if (isCtrlC && m.Msg == WM_CLIPBOARDUPDATE)
            {
                // Clipboard updated (copy event) - CTRL+C+C detection
                var now = DateTime.Now;
                var timeSinceLastCopy = (now - lastClipboardCopy).TotalMilliseconds;
                if (timeSinceLastCopy <= DOUBLE_KEY_TIMEOUT_MS)
                {
                    lastClipboardCopy = DateTime.MinValue; // Reset
                    OpenWebsite(); // Trigger translation
                }
                else
                {
                    lastClipboardCopy = now;
                }
            }
            else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == hotKeyId)
            {
                // Global hotkey pressed (for non-CTRL+C combinations)
                OpenWebsite();
            }
            base.WndProc(ref m);
        }

        private void OnOpenWebsite(object sender, EventArgs e)
        {
            OpenWebsite();
        }

        private void OpenWebsite()
        {
            try
            {
                // Get text from clipboard for translation
                string clipboardText = GetClipboardText();
                if (string.IsNullOrEmpty(clipboardText))
                {
                    MessageBox.Show("No text found in clipboard to translate.", 
                        "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Auto-switch languages if enabled (but skip when source language is "auto" - Detect language)
                if (settings.AutoSwitchLanguages && settings.SourceLanguage != "auto")
                {
                    var (newSource, newTarget) = LanguageDetector.GetAutoSwitchedLanguages(
                        clipboardText, settings.SourceLanguage, settings.TargetLanguage);
                    
                    if (newSource != settings.SourceLanguage || newTarget != settings.TargetLanguage)
                    {
                        settings.SourceLanguage = newSource;
                        settings.TargetLanguage = newTarget;
                        settings.Save();
                        BuildTrayMenu(); // Update menu to reflect language changes
                        UpdateTrayIconText();
                    }
                }

                string url;

                if (settings.PreferredTranslator == TranslatorType.TranslationServer)
                {
                    url = $"{settings.TranslationServerUrl}/translation-server.html?sl={settings.SourceLanguage}&tl={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}&server={Uri.EscapeDataString(settings.TranslationServerUrl)}&token={Uri.EscapeDataString(settings.TranslationServerToken)}";
                    // If window is open, close it before opening a new one
                    if (translationServerWindowProcess != null && !translationServerWindowProcess.HasExited)
                    {
                        try { translationServerWindowProcess.Kill(); translationServerWindowProcess.WaitForExit(2000); } catch { }
                        translationServerWindowProcess = null;
                    }
                }
                else if (settings.PreferredTranslator == TranslatorType.Google)
                {
                    url = $"https://translate.google.com/?sl={settings.SourceLanguage}&tl={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}&op=translate";
                }
                else if (settings.PreferredTranslator == TranslatorType.ChatGPT)
                {
                    url = $"{settings.TranslationServerUrl}/translator.html?sl={settings.SourceLanguage}&tl={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}&apikey={Uri.EscapeDataString(settings.ChatGptApiKey)}";
                    // If window is open, close it before opening a new one
                    if (chatGptWindowProcess != null && !chatGptWindowProcess.HasExited)
                    {
                        try { chatGptWindowProcess.Kill(); chatGptWindowProcess.WaitForExit(2000); } catch { }
                        chatGptWindowProcess = null;
                    }
                }
                else
                {
                    url = $"https://translate.yandex.com/?source_lang={settings.SourceLanguage}&target_lang={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}";
                }

                // Try to open in app mode (minimal UI) with Chrome or Edge
                Process launchedProcess = null;
                if (!TryOpenInAppModeWithProcess(url, out launchedProcess))
                {
                    // Fallback to default browser if app mode fails
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else
                {
                    // Track the process for ChatGPT and Translation Server
                    if (settings.PreferredTranslator == TranslatorType.ChatGPT)
                        chatGptWindowProcess = launchedProcess;
                    else if (settings.PreferredTranslator == TranslatorType.TranslationServer)
                        translationServerWindowProcess = launchedProcess;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open: {ex.Message}",
                    "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetClipboardText()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    return Clipboard.GetText().Trim();
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private bool TryOpenInAppModeWithProcess(string url, out Process launchedProcess)
        {
            string tempUserDataDir = Path.Combine(Path.GetTempPath(), "MiniTranslate_" + Guid.NewGuid().ToString("N"));
            launchedProcess = null;
            switch (settings.PreferredBrowser)
            {
                case BrowserType.Chrome:
                    return TryOpenWithChrome(url, tempUserDataDir, out launchedProcess);
                case BrowserType.Edge:
                    return TryOpenWithEdge(url, tempUserDataDir, out launchedProcess);
                case BrowserType.Default:
                    return false;
                default:
                    return TryOpenWithChrome(url, tempUserDataDir, out launchedProcess) || TryOpenWithEdge(url, tempUserDataDir, out launchedProcess);
            }
        }

        private bool TryOpenWithChrome(string url, string userDataDir, out Process launchedProcess)
        {
            var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
            var userDataArgs = $"--user-data-dir=\"{userDataDir}\"";
            var arguments = $"--app={url} {sizeArgs} {userDataArgs} --disable-extensions";
            launchedProcess = null;
            if (TryStartProcess("chrome.exe", arguments, out launchedProcess))
                return true;
            string[] chromePaths = {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe")
            };
            foreach (var chromePath in chromePaths)
            {
                if (File.Exists(chromePath) && TryStartProcess(chromePath, arguments, out launchedProcess))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryOpenWithEdge(string url, string userDataDir, out Process launchedProcess)
        {
            var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
            var userDataArgs = $"--user-data-dir=\"{userDataDir}\"";
            var arguments = $"--app={url} {sizeArgs} {userDataArgs} --disable-extensions";
            launchedProcess = null;
            if (TryStartProcess("msedge.exe", arguments, out launchedProcess))
                return true;
            string[] edgePaths = {
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };
            foreach (var edgePath in edgePaths)
            {
                if (File.Exists(edgePath) && TryStartProcess(edgePath, arguments, out launchedProcess))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryStartProcess(string fileName, string arguments, out Process launchedProcess)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                launchedProcess = Process.Start(processStartInfo);
                return launchedProcess != null;
            }
            catch
            {
                launchedProcess = null;
                return false;
            }
        }

        private void UpdateTrayIcon()
        {
            if (trayIcon != null)
            {
                var newIcon = LoadCustomIcon();
                if (trayIcon.Icon != newIcon)
                {
                    trayIcon.Icon?.Dispose();
                    trayIcon.Icon = newIcon;
                }
            }
        }

        private void OnTranslatorChanged(object sender, EventArgs e)
        {
            var selectedItem = sender as ToolStripMenuItem;
            if (selectedItem?.Tag is TranslatorType selectedTranslator)
            {
                settings.PreferredTranslator = selectedTranslator;
                settings.Save();
                UpdateTrayIconText();
                BuildTrayMenu(); // Rebuild menu to update checkmarks
            }
        }

        private void OnBrowserChanged(object sender, EventArgs e)
        {
            var selectedItem = sender as ToolStripMenuItem;
            if (selectedItem?.Tag is BrowserType selectedBrowser)
            {
                settings.PreferredBrowser = selectedBrowser;
                settings.Save();
                BuildTrayMenu(); // Rebuild menu to update checkmarks
            }
        }

        private void OnSourceLanguageChanged(object sender, EventArgs e)
        {
            var selectedItem = sender as ToolStripMenuItem;
            if (selectedItem?.Tag is string langCode)
            {
                settings.SourceLanguage = langCode;
                settings.Save();
                UpdateTrayIconText();
                BuildTrayMenu();
            }
        }

        private void OnTargetLanguageChanged(object sender, EventArgs e)
        {
            var selectedItem = sender as ToolStripMenuItem;
            if (selectedItem?.Tag is string langCode)
            {
                settings.TargetLanguage = langCode;
                settings.Save();
                UpdateTrayIconText();
                BuildTrayMenu();
            }
        }

        private void OnSettings(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(settings))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    settings = settingsForm.Settings;
                    settings.Save();
                    UnregisterHotKey(this.Handle, hotKeyId);
                    StopClipboardListener();
                    
                    // Determine hotkey type based on key combination
                    // If CTRL+C is selected, use double press (clipboard monitoring)
                    // Otherwise, use single press (global hotkey)
                    bool isCtrlC = (settings.HotkeyModifiers & MOD_CONTROL) != 0 && settings.HotkeyKey == (int)Keys.C;
                    
                    if (isCtrlC)
                    {
                        StartClipboardListener();
                    }
                    else
                    {
                        RegisterGlobalHotKey();
                    }
                    
                    UpdateTrayIconText();
                    UpdateTrayIcon();
                    BuildTrayMenu(); // Rebuild menu to reflect changes
                }
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, hotKeyId);
            StopClipboardListener();
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnregisterHotKey(this.Handle, hotKeyId);
                StopClipboardListener();
                trayIcon?.Dispose();
                trayMenu?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }



        private void OnSwitchLanguages(object sender, EventArgs e)
        {
            // Swap source and target languages
            var temp = settings.SourceLanguage;
            settings.SourceLanguage = settings.TargetLanguage;
            settings.TargetLanguage = temp;
            settings.Save();
            BuildTrayMenu();
        }



        private void StartClipboardListener()
        {
            if (!clipboardListenerActive)
            {
                AddClipboardFormatListener(this.Handle);
                clipboardListenerActive = true;
            }
        }
        private void StopClipboardListener()
        {
            if (clipboardListenerActive)
            {
                RemoveClipboardFormatListener(this.Handle);
                clipboardListenerActive = false;
            }
        }
    }
} 