using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MiniTranslator
{
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
            InitializeTrayIcon();
            LoadSettings();
            RegisterGlobalHotKey();
            
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
            trayMenu.Items.Add("Open/Translate", null, OnOpenWebsite);
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Settings", null, OnSettings);
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Exit", null, OnExit);

            trayIcon = new NotifyIcon()
            {
                Text = "MiniTranslator - Quick Translator & Website Launcher",
                Icon = LoadCustomIcon(),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += OnOpenWebsite;
        }

        private Icon LoadCustomIcon()
        {
            // Try to load icon from specific path first
            try
            {
                string iconPath = @"C:\Users\USER\Documents\GitHub\minibrowser\icon.ico";
                if (File.Exists(iconPath))
                {
                    return new Icon(iconPath);
                }
            }
            catch
            {
                // Fall back to relative path
                try
                {
                    if (File.Exists("icon.ico"))
                    {
                        return new Icon("icon.ico");
                    }
                }
                catch
                {
                    // Fall back to default if both fail
                }
            }
            
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
            UpdateTrayIconText();
        }

        private void UpdateTrayIconText()
        {
            var hotkeyText = GetHotkeyDisplayString();
            if (settings.UseTranslateMode)
            {
                trayIcon.Text = $"MiniTranslator - {hotkeyText} to translate clipboard ({settings.SourceLanguage}→{settings.TargetLanguage})";
            }
            else
            {
                trayIcon.Text = $"MiniTranslator - {hotkeyText} to open custom website: {settings.WebsiteUrl}";
            }
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
                string url;
                
                if (settings.UseTranslateMode)
                {
                    // Get text from clipboard for translation
                    string clipboardText = GetClipboardText();
                    if (string.IsNullOrEmpty(clipboardText))
                    {
                        MessageBox.Show("No text found in clipboard to translate.", 
                            "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Construct Yandex Translate URL with clipboard text
                    url = $"https://translate.yandex.com/?source_lang={settings.SourceLanguage}&target_lang={settings.TargetLanguage}&text={Uri.EscapeDataString(clipboardText)}";
                }
                else
                {
                    // Custom website mode
                    if (string.IsNullOrWhiteSpace(settings.WebsiteUrl))
                    {
                        MessageBox.Show("No custom website URL configured. Please check settings.",
                            "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    url = settings.WebsiteUrl;
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "https://" + url;
                    }
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
            switch (settings.PreferredBrowser)
            {
                case BrowserType.Chrome:
                    return TryOpenWithChrome(url);
                    
                case BrowserType.Edge:
                    return TryOpenWithEdge(url);
                    
                case BrowserType.Default:
                    // Use default browser (system default)
                    return false; // Will trigger fallback to default browser
                    
                default:
                    // Fallback to old behavior (try Chrome first, then Edge)
                    return TryOpenWithChrome(url) || TryOpenWithEdge(url);
            }
        }

        private bool TryOpenWithChrome(string url)
        {
            // Try Chrome with generic name first
            if (TryOpenWithBrowser(url, "chrome.exe", "--app="))
                return true;

            // Try Chrome in common installation paths
            string[] chromePaths = {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe")
            };

            foreach (var chromePath in chromePaths)
            {
                if (File.Exists(chromePath))
                {
                    return TryStartProcess(chromePath, $"--app={url} --new-window");
                }
            }

            return false;
        }

        private bool TryOpenWithEdge(string url)
        {
            // Try Edge with generic name first
            if (TryOpenWithBrowser(url, "msedge.exe", "--app="))
                return true;

            // Try Edge in common installation paths
            string[] edgePaths = {
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (var edgePath in edgePaths)
            {
                if (File.Exists(edgePath))
                {
                    return TryStartProcess(edgePath, $"--app={url} --new-window");
                }
            }

            return false;
        }

        private bool TryOpenWithBrowser(string url, string browserExecutable, string appFlag)
        {
            try
            {
                // Add window size parameters
                var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
                var arguments = $"{appFlag}{url} --new-window {sizeArgs}";

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = browserExecutable,
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

        private bool TryStartProcess(string fileName, string arguments)
        {
            try
            {
                // Add window size parameters
                var sizeArgs = $"--window-size={settings.WindowWidth},{settings.WindowHeight}";
                var fullArguments = $"{arguments} {sizeArgs}";

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = fullArguments,
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