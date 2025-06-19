using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MiniTranslator
{
    public static class LanguageProvider
    {
        public static Dictionary<string, string> GetLanguages()
        {
            return new Dictionary<string, string>
            {
                {"en", "English"}, {"ru", "Русский"}, {"es", "Español"}, {"fr", "Français"},
                {"de", "Deutsch"}, {"it", "Italiano"}, {"pt", "Português"}, {"zh", "中文"},
                {"ja", "日本語"}, {"ko", "한국어"}, {"ar", "العربية"}, {"hi", "हिन्दी"},
                {"tr", "Türkçe"}, {"pl", "Polski"}, {"nl", "Nederlands"}, {"sv", "Svenska"},
                {"da", "Dansk"}, {"no", "Norsk"}, {"fi", "Suomi"}, {"cs", "Čeština"},
                {"uk", "Українська"}, {"bg", "Български"}, {"hr", "Hrvatski"}, {"sk", "Slovenčina"},
                {"sl", "Slovenščina"}, {"et", "Eesti"}, {"lv", "Latviešu"}, {"lt", "Lietuvių"},
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

        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
            InitializeTrayIcon();
            RegisterGlobalHotKey();
            UpdateTrayIconText();
            
            // Hide the form initially
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;
        }

        private void InitializeComponent()
        {
            this.Text = "MiniTranslator";
            this.Size = new Size(1, 1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayIcon = new NotifyIcon()
            {
                Text = "MiniTranslator", // Set initial placeholder text
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

            // Translator Service Menu
            var translatorMenu = new ToolStripMenuItem("Translator");
            var yandexItem = new ToolStripMenuItem("Yandex Translate", null, OnTranslatorChanged) { Tag = TranslatorType.Yandex };
            var googleItem = new ToolStripMenuItem("Google Translate", null, OnTranslatorChanged) { Tag = TranslatorType.Google };
            yandexItem.Checked = settings.PreferredTranslator == TranslatorType.Yandex;
            googleItem.Checked = settings.PreferredTranslator == TranslatorType.Google;
            translatorMenu.DropDownItems.Add(yandexItem);
            translatorMenu.DropDownItems.Add(googleItem);
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

                // Add to Target Language Menu
                var targetItem = new ToolStripMenuItem(lang.Value, null, OnTargetLanguageChanged) { Tag = lang.Key };
                targetItem.Checked = settings.TargetLanguage == lang.Key;
                targetLangMenu.DropDownItems.Add(targetItem);
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
            var translatorName = settings.PreferredTranslator == TranslatorType.Google ? "Google" : "Yandex";
            trayIcon.Text = $"MiniTranslator - {hotkeyText} to translate clipboard ({settings.SourceLanguage}→{settings.TargetLanguage}) via {translatorName}";
        }

        private string GetHotkeyDisplayString()
        {
            var parts = new List<string>();
            
            if ((settings.HotkeyModifiers & MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((settings.HotkeyModifiers & MOD_ALT) != 0) parts.Add("Alt");
            if ((settings.HotkeyModifiers & MOD_SHIFT) != 0) parts.Add("Shift");
            if ((settings.HotkeyModifiers & MOD_WIN) != 0) parts.Add("Win");
            
            parts.Add(((Keys)settings.HotkeyKey).ToString());
            
            return string.Join("+", parts);
        }

        private void RegisterGlobalHotKey()
        {
            UnregisterHotKey(this.Handle, hotKeyId);
            
            if (!RegisterHotKey(this.Handle, hotKeyId, settings.HotkeyModifiers, settings.HotkeyKey))
            {
                MessageBox.Show($"Failed to register hotkey {GetHotkeyDisplayString()}. It might be already in use.",
                    "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            // Update tray icon if settings changed
            UpdateTrayIcon();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == hotKeyId)
            {
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
                        "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string url;
                if (settings.PreferredTranslator == TranslatorType.Google)
                {
                    // Google Translate URL format
                    url = $"https://translate.google.com/?sl={settings.SourceLanguage}&tl={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}&op=translate";
                }
                else
                {
                    // Yandex Translate URL format
                    url = $"https://translate.yandex.com/?source_lang={settings.SourceLanguage}&target_lang={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}";
                }

                // Try to open in app mode (minimal UI) with Chrome or Edge
                if (!TryOpenInAppMode(url))
                {
                    // Fallback to default browser if app mode fails
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open: {ex.Message}",
                    "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool TryOpenInAppMode(string url)
        {
            // Generate a temporary user data directory for a clean session
            string tempUserDataDir = Path.Combine(Path.GetTempPath(), "MiniTranslator_" + Guid.NewGuid().ToString("N"));

            switch (settings.PreferredBrowser)
            {
                case BrowserType.Chrome:
                    return TryOpenWithChrome(url, tempUserDataDir);
                    
                case BrowserType.Edge:
                    return TryOpenWithEdge(url, tempUserDataDir);
                    
                case BrowserType.Default:
                    // Default browser doesn't support app mode or custom user directories
                    return false; 
                    
                default:
                    // Fallback to old behavior (try Chrome first, then Edge)
                    return TryOpenWithChrome(url, tempUserDataDir) || TryOpenWithEdge(url, tempUserDataDir);
            }
        }

        private bool TryOpenWithChrome(string url, string userDataDir)
        {
            var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
            var userDataArgs = $"--user-data-dir=\"{userDataDir}\"";
            var arguments = $"--app={url} {sizeArgs} {userDataArgs} --disable-extensions";

            // Try Chrome with generic name first
            if (TryStartProcess("chrome.exe", arguments))
                return true;

            // Try Chrome in common installation paths
            string[] chromePaths = {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe")
            };

            foreach (var chromePath in chromePaths)
            {
                if (File.Exists(chromePath) && TryStartProcess(chromePath, arguments))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryOpenWithEdge(string url, string userDataDir)
        {
            var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
            var userDataArgs = $"--user-data-dir=\"{userDataDir}\"";
            var arguments = $"--app={url} {sizeArgs} {userDataArgs} --disable-extensions";
            
            // Try Edge with generic name first
            if (TryStartProcess("msedge.exe", arguments))
                return true;

            // Try Edge in common installation paths
            string[] edgePaths = {
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (var edgePath in edgePaths)
            {
                if (File.Exists(edgePath) && TryStartProcess(edgePath, arguments))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryStartProcess(string fileName, string arguments)
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

                Process.Start(processStartInfo);
                return true;
            }
            catch
            {
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
                    RegisterGlobalHotKey();
                    UpdateTrayIconText();
                    UpdateTrayIcon();
                    BuildTrayMenu(); // Rebuild menu to reflect changes
                }
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, hotKeyId);
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnregisterHotKey(this.Handle, hotKeyId);
                trayIcon?.Dispose();
                trayMenu?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }
    }
} 