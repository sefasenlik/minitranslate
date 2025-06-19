using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace MiniTranslator
{
    public partial class SettingsForm : Form
    {
        public AppSettings Settings { get; private set; }

        private CheckBox ctrlCheckBox;
        private CheckBox altCheckBox;
        private CheckBox shiftCheckBox;
        private CheckBox winCheckBox;
        private ComboBox keyComboBox;
        private ComboBox sourceLanguageCombo;
        private ComboBox targetLanguageCombo;
        private ComboBox translatorComboBox;
        private TrackBar widthSlider;
        private TrackBar heightSlider;
        private Label widthLabel;
        private Label heightLabel;
        private ComboBox browserComboBox;
        private Button okButton;
        private Button cancelButton;
        private Button testButton;
        private CheckBox startupCheckBox;

        // Hotkey modifiers
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        private const string AppName = "MiniTranslator";
        private static readonly RegistryKey StartupRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public SettingsForm(AppSettings currentSettings)
        {
            Settings = new AppSettings
            {
                HotkeyModifiers = currentSettings.HotkeyModifiers,
                HotkeyKey = currentSettings.HotkeyKey,
                SourceLanguage = currentSettings.SourceLanguage,
                TargetLanguage = currentSettings.TargetLanguage,
                PreferredTranslator = currentSettings.PreferredTranslator,
                WindowWidth = currentSettings.WindowWidth,
                WindowHeight = currentSettings.WindowHeight,
                PreferredBrowser = currentSettings.PreferredBrowser
            };

            InitializeComponent();
            LoadCurrentSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "MiniTranslator Settings";
            this.Size = new Size(490, 460);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Translator selection
            var translatorLabel = new Label
            {
                Text = "Translation Service:",
                Location = new Point(20, 20),
                Size = new Size(120, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            this.Controls.Add(translatorLabel);

            translatorComboBox = new ComboBox
            {
                Location = new Point(145, 20),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            translatorComboBox.Items.Add("Yandex Translate");
            translatorComboBox.Items.Add("Google Translate");
            this.Controls.Add(translatorComboBox);

            // Translation settings
            var sourceLabel = new Label
            {
                Text = "Source Language:",
                Location = new Point(20, 55),
                Size = new Size(110, 23)
            };
            this.Controls.Add(sourceLabel);

            sourceLanguageCombo = new ComboBox
            {
                Location = new Point(135, 55),
                Size = new Size(85, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(sourceLanguageCombo);

            var targetLabel = new Label
            {
                Text = "Target Language:",
                Location = new Point(235, 55),
                Size = new Size(110, 23)
            };
            this.Controls.Add(targetLabel);

            targetLanguageCombo = new ComboBox
            {
                Location = new Point(350, 55),
                Size = new Size(85, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(targetLanguageCombo);

            // Populate language combo boxes after they're created and added
            PopulateLanguageComboBoxes();

            // Test button
            testButton = new Button
            {
                Text = "Test Translation",
                Location = new Point(20, 340),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(225, 240, 255),
                FlatStyle = FlatStyle.Flat,
            };
            testButton.FlatAppearance.BorderColor = Color.FromArgb(173, 216, 230);
            testButton.FlatAppearance.BorderSize = 1;
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);

            // Hotkey section
            var hotkeyLabel = new Label
            {
                Text = "Global Hotkey:",
                Location = new Point(20, 130),
                Size = new Size(100, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            this.Controls.Add(hotkeyLabel);

            // Modifier checkboxes
            ctrlCheckBox = new CheckBox
            {
                Text = "Ctrl",
                Location = new Point(30, 155),
                Size = new Size(50, 23)
            };
            ctrlCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            this.Controls.Add(ctrlCheckBox);

            altCheckBox = new CheckBox
            {
                Text = "Alt",
                Location = new Point(85, 155),
                Size = new Size(45, 23)
            };
            altCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            this.Controls.Add(altCheckBox);

            shiftCheckBox = new CheckBox
            {
                Text = "Shift",
                Location = new Point(135, 155),
                Size = new Size(50, 23)
            };
            shiftCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            this.Controls.Add(shiftCheckBox);

            winCheckBox = new CheckBox
            {
                Text = "Win",
                Location = new Point(190, 155),
                Size = new Size(55, 23)
            };
            winCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            this.Controls.Add(winCheckBox);

            // Key selection
            var keyLabel = new Label
            {
                Text = "Key:",
                Location = new Point(250, 155),
                Size = new Size(30, 23)
            };
            this.Controls.Add(keyLabel);

            keyComboBox = new ComboBox
            {
                Location = new Point(285, 155),
                Size = new Size(60, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            keyComboBox.SelectedIndexChanged += UpdateCurrentHotkeyDisplay;
            this.Controls.Add(keyComboBox);

            PopulateKeyComboBox();

            // Browser selection
            var browserLabel = new Label
            {
                Text = "Preferred Browser:",
                Location = new Point(20, 190),
                Size = new Size(120, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            this.Controls.Add(browserLabel);

            browserComboBox = new ComboBox
            {
                Location = new Point(145, 190),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            browserComboBox.Items.Add("Chrome");
            browserComboBox.Items.Add("Edge");
            browserComboBox.Items.Add("Default Browser");
            this.Controls.Add(browserComboBox);

            // Window Size section
            var windowSizeLabel = new Label
            {
                Text = "Browser Window Size:",
                Location = new Point(20, 225),
                Size = new Size(150, 23)
            };
            this.Controls.Add(windowSizeLabel);

            // Width slider
            var widthSizeLabel = new Label
            {
                Text = "Width:",
                Location = new Point(20, 250),
                Size = new Size(50, 23)
            };
            this.Controls.Add(widthSizeLabel);

            widthSlider = new TrackBar
            {
                Location = new Point(75, 250),
                Size = new Size(200, 45),
                Minimum = 300,
                Maximum = 1920,
                Value = 1200,
                TickFrequency = 100
            };
            widthSlider.ValueChanged += WidthSlider_ValueChanged;
            this.Controls.Add(widthSlider);

            widthLabel = new Label
            {
                Text = "1200px",
                Location = new Point(285, 260),
                Size = new Size(60, 23)
            };
            this.Controls.Add(widthLabel);

            // Height slider
            var heightSizeLabel = new Label
            {
                Text = "Height:",
                Location = new Point(20, 290),
                Size = new Size(50, 23)
            };
            this.Controls.Add(heightSizeLabel);

            heightSlider = new TrackBar
            {
                Location = new Point(75, 290),
                Size = new Size(200, 45),
                Minimum = 300,
                Maximum = 1080,
                Value = 800,
                TickFrequency = 50
            };
            heightSlider.ValueChanged += HeightSlider_ValueChanged;
            this.Controls.Add(heightSlider);

            heightLabel = new Label
            {
                Text = "800px",
                Location = new Point(285, 300),
                Size = new Size(60, 23)
            };
            this.Controls.Add(heightLabel);

            // Current hotkey display
            var currentLabel = new Label
            {
                Text = "Current: Ctrl+Q",
                Location = new Point(355, 155),
                Size = new Size(100, 20),
                ForeColor = Color.Blue
            };
            currentLabel.Name = "currentLabel";
            this.Controls.Add(currentLabel);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(295, 340),
                Size = new Size(75, 30)
            };
            okButton.DialogResult = DialogResult.OK;
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(380, 340),
                Size = new Size(75, 30)
            };
            cancelButton.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            // Startup Checkbox
            startupCheckBox = new CheckBox
            {
                Text = "Run at Windows startup",
                Location = new Point(20, 380),
                Size = new Size(180, 23)
            };
            this.Controls.Add(startupCheckBox);

            // Load initial state
            LoadStartupState();
        }

        private void PopulateKeyComboBox()
        {
            var commonKeys = new[]
            {
                Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P,
                Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L,
                Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M,
                Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6,
                Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12,
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0
            };

            foreach (var key in commonKeys)
            {
                keyComboBox.Items.Add(key);
            }
        }

        private void PopulateLanguageComboBoxes()
        {
            var languages = LanguageProvider.GetLanguages();
            foreach (var lang in languages)
            {
                var item = new LanguageItem { Code = lang.Key, Name = lang.Value };
                sourceLanguageCombo.Items.Add(item);
                targetLanguageCombo.Items.Add(item);
            }
        }

        private class LanguageItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void LoadCurrentSettings()
        {
            // Load translator selection
            translatorComboBox.SelectedIndex = (int)Settings.PreferredTranslator;
            
            // Load browser selection
            browserComboBox.SelectedIndex = (int)Settings.PreferredBrowser;
            
            widthSlider.Value = Math.Max(widthSlider.Minimum, Math.Min(widthSlider.Maximum, Settings.WindowWidth));
            heightSlider.Value = Math.Max(heightSlider.Minimum, Math.Min(heightSlider.Maximum, Settings.WindowHeight));
            
            UpdateSliderLabels();
            
            // Load language settings
            foreach (LanguageItem item in sourceLanguageCombo.Items)
            {
                if (item.Code == Settings.SourceLanguage)
                {
                    sourceLanguageCombo.SelectedItem = item;
                    break;
                }
            }
            
            foreach (LanguageItem item in targetLanguageCombo.Items)
            {
                if (item.Code == Settings.TargetLanguage)
                {
                    targetLanguageCombo.SelectedItem = item;
                    break;
                }
            }
            
            ctrlCheckBox.Checked = (Settings.HotkeyModifiers & MOD_CONTROL) != 0;
            altCheckBox.Checked = (Settings.HotkeyModifiers & MOD_ALT) != 0;
            shiftCheckBox.Checked = (Settings.HotkeyModifiers & MOD_SHIFT) != 0;
            winCheckBox.Checked = (Settings.HotkeyModifiers & MOD_WIN) != 0;
            
            keyComboBox.SelectedItem = (Keys)Settings.HotkeyKey;
            
            // Load browser setting
            browserComboBox.SelectedIndex = (int)Settings.PreferredBrowser;
            
            UpdateCurrentHotkeyDisplay(null, null);
        }

        private void UpdateCurrentHotkeyDisplay(object sender, EventArgs e)
        {
            var currentLabel = this.Controls.Find("currentLabel", false)[0] as Label;
            if (currentLabel != null)
            {
                var parts = new System.Collections.Generic.List<string>();
                
                if (ctrlCheckBox.Checked) parts.Add("Ctrl");
                if (altCheckBox.Checked) parts.Add("Alt");
                if (shiftCheckBox.Checked) parts.Add("Shift");
                if (winCheckBox.Checked) parts.Add("Win");
                
                if (keyComboBox.SelectedItem != null)
                {
                    parts.Add(keyComboBox.SelectedItem.ToString());
                }
                
                currentLabel.Text = parts.Count > 0 ? $"Current: {string.Join("+", parts)}" : "Current: (none)";
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Test translation with clipboard text
                string clipboardText = GetClipboardText();
                if (string.IsNullOrEmpty(clipboardText))
                {
                    MessageBox.Show("No text found in clipboard to test translation. Copy some text and try again.", 
                        "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var sourceItem = sourceLanguageCombo.SelectedItem as LanguageItem;
                var targetItem = targetLanguageCombo.SelectedItem as LanguageItem;
                
                if (sourceItem == null || targetItem == null)
                {
                    MessageBox.Show("Please select both source and target languages.", 
                        "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string url;
                var selectedTranslator = (TranslatorType)translatorComboBox.SelectedIndex;
                
                if (selectedTranslator == TranslatorType.Google)
                {
                    // Google Translate URL format
                    url = $"https://translate.google.com/?sl={sourceItem.Code}&tl={targetItem.Code}&text={Uri.EscapeDataString(clipboardText)}&op=translate";
                }
                else
                {
                    // Yandex Translate URL format
                    url = $"https://translate.yandex.com/?source_lang={sourceItem.Code}&target_lang={targetItem.Code}&text={Uri.EscapeDataString(clipboardText)}";
                }
                
                // Try to open in app mode
                if (!TryOpenInAppMode(url))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) 
                    { 
                        UseShellExecute = true 
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open: {ex.Message}", "MiniTranslator", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void WidthSlider_ValueChanged(object sender, EventArgs e)
        {
            UpdateSliderLabels();
        }

        private void HeightSlider_ValueChanged(object sender, EventArgs e)
        {
            UpdateSliderLabels();
        }

        private void UpdateSliderLabels()
        {
            widthLabel.Text = $"{widthSlider.Value}px";
            heightLabel.Text = $"{heightSlider.Value}px";
        }

        private bool TryOpenInAppMode(string url)
        {
            var selectedBrowser = (BrowserType)browserComboBox.SelectedIndex;
            
            switch (selectedBrowser)
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
                System.Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe")
            };

            foreach (var chromePath in chromePaths)
            {
                if (System.IO.File.Exists(chromePath))
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
                if (System.IO.File.Exists(edgePath))
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
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = browserExecutable,
                    Arguments = $"{appFlag}{url} --new-window",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                System.Diagnostics.Process.Start(processStartInfo);
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
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                System.Diagnostics.Process.Start(processStartInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void LoadStartupState()
        {
            if (StartupRegistryKey?.GetValue(AppName) != null)
            {
                startupCheckBox.Checked = true;
            }
            else
            {
                startupCheckBox.Checked = false;
            }
        }

        private void SetStartup(bool enable)
        {
            if (StartupRegistryKey == null) return;

            if (enable)
            {
                // Set the application to run at startup
                StartupRegistryKey.SetValue(AppName, Application.ExecutablePath);
            }
            else
            {
                // Remove the application from startup
                StartupRegistryKey.DeleteValue(AppName, false);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Validate translation settings
            if (sourceLanguageCombo.SelectedItem == null || targetLanguageCombo.SelectedItem == null)
            {
                MessageBox.Show("Please select both source and target languages.", 
                    "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (translatorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a translation service.", 
                    "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate hotkey
            if (keyComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a key for the hotkey.", "MiniTranslator", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ctrlCheckBox.Checked && !altCheckBox.Checked && !shiftCheckBox.Checked && !winCheckBox.Checked)
            {
                MessageBox.Show("Please select at least one modifier key (Ctrl, Alt, Shift, or Win).", 
                    "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save settings
            Settings.PreferredTranslator = (TranslatorType)translatorComboBox.SelectedIndex;
            Settings.WindowWidth = widthSlider.Value;
            Settings.WindowHeight = heightSlider.Value;
            
            var sourceItem = sourceLanguageCombo.SelectedItem as LanguageItem;
            var targetItem = targetLanguageCombo.SelectedItem as LanguageItem;
            Settings.SourceLanguage = sourceItem.Code;
            Settings.TargetLanguage = targetItem.Code;
            
            Settings.HotkeyModifiers = 0;
            if (ctrlCheckBox.Checked) Settings.HotkeyModifiers |= MOD_CONTROL;
            if (altCheckBox.Checked) Settings.HotkeyModifiers |= MOD_ALT;
            if (shiftCheckBox.Checked) Settings.HotkeyModifiers |= MOD_SHIFT;
            if (winCheckBox.Checked) Settings.HotkeyModifiers |= MOD_WIN;
            
            Settings.HotkeyKey = (int)(Keys)keyComboBox.SelectedItem;
            
            // Save browser setting
            Settings.PreferredBrowser = (BrowserType)browserComboBox.SelectedIndex;

            // Handle Startup setting
            SetStartup(startupCheckBox.Checked);
        }
    }
} 