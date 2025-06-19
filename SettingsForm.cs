using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniTranslator
{
    public partial class SettingsForm : Form
    {
        public AppSettings Settings { get; private set; }

        private TextBox urlTextBox;
        private CheckBox ctrlCheckBox;
        private CheckBox altCheckBox;
        private CheckBox shiftCheckBox;
        private CheckBox winCheckBox;
        private ComboBox keyComboBox;
        private ComboBox sourceLanguageCombo;
        private ComboBox targetLanguageCombo;
        private CheckBox translateModeCheckBox;
        private TrackBar widthSlider;
        private TrackBar heightSlider;
        private Label widthLabel;
        private Label heightLabel;
        private ComboBox browserComboBox;
        private Button okButton;
        private Button cancelButton;
        private Button testButton;

        // Hotkey modifiers
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        public SettingsForm(AppSettings currentSettings)
        {
            Settings = new AppSettings
            {
                WebsiteUrl = currentSettings.WebsiteUrl,
                HotkeyModifiers = currentSettings.HotkeyModifiers,
                HotkeyKey = currentSettings.HotkeyKey,
                SourceLanguage = currentSettings.SourceLanguage,
                TargetLanguage = currentSettings.TargetLanguage,
                UseTranslateMode = currentSettings.UseTranslateMode,
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
            this.Size = new Size(490, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Translation mode checkbox
            translateModeCheckBox = new CheckBox
            {
                Text = "Translation Mode (translates clipboard text via Yandex)",
                Location = new Point(20, 20),
                Size = new Size(340, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            translateModeCheckBox.CheckedChanged += TranslateModeCheckBox_CheckedChanged;
            this.Controls.Add(translateModeCheckBox);

            // Translation settings
            var sourceLabel = new Label
            {
                Text = "Source Language:",
                Location = new Point(40, 50),
                Size = new Size(110, 23)
            };
            this.Controls.Add(sourceLabel);

            sourceLanguageCombo = new ComboBox
            {
                Location = new Point(155, 50),
                Size = new Size(85, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(sourceLanguageCombo);

            var targetLabel = new Label
            {
                Text = "Target Language:",
                Location = new Point(250, 50),
                Size = new Size(110, 23)
            };
            this.Controls.Add(targetLabel);

            targetLanguageCombo = new ComboBox
            {
                Location = new Point(365, 50),
                Size = new Size(85, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(targetLanguageCombo);

            // Populate language combo boxes after they're created and added
            PopulateLanguageComboBoxes();

            // Website URL (for custom website mode)
            var urlLabel = new Label
            {
                Text = "Custom Website URL:",
                Location = new Point(20, 90),
                Size = new Size(130, 23)
            };
            this.Controls.Add(urlLabel);

            urlTextBox = new TextBox
            {
                Location = new Point(20, 115),
                Size = new Size(360, 23),
                PlaceholderText = "https://www.example.com (for Custom Website Mode)"
            };
            this.Controls.Add(urlTextBox);

            // Test button
            testButton = new Button
            {
                Text = "Test",
                Location = new Point(305, 145),
                Size = new Size(75, 25)
            };
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);

            // Window Size section
            var windowSizeLabel = new Label
            {
                Text = "Browser Window Size:",
                Location = new Point(20, 180),
                Size = new Size(150, 23)
            };
            this.Controls.Add(windowSizeLabel);

            // Width slider
            var widthSizeLabel = new Label
            {
                Text = "Width:",
                Location = new Point(20, 205),
                Size = new Size(50, 23)
            };
            this.Controls.Add(widthSizeLabel);

            widthSlider = new TrackBar
            {
                Location = new Point(75, 205),
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
                Location = new Point(285, 215),
                Size = new Size(60, 23)
            };
            this.Controls.Add(widthLabel);

            // Height slider
            var heightSizeLabel = new Label
            {
                Text = "Height:",
                Location = new Point(20, 245),
                Size = new Size(50, 23)
            };
            this.Controls.Add(heightSizeLabel);

            heightSlider = new TrackBar
            {
                Location = new Point(75, 245),
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
                Location = new Point(285, 255),
                Size = new Size(60, 23)
            };
            this.Controls.Add(heightLabel);

            // Browser selection
            var browserLabel = new Label
            {
                Text = "Preferred Browser:",
                Location = new Point(20, 295),
                Size = new Size(120, 23)
            };
            this.Controls.Add(browserLabel);

            browserComboBox = new ComboBox
            {
                Location = new Point(150, 295),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            browserComboBox.Items.Add("Chrome");
            browserComboBox.Items.Add("Edge");
            browserComboBox.Items.Add("Default Browser");
            this.Controls.Add(browserComboBox);

            // Hotkey section
            var hotkeyLabel = new Label
            {
                Text = "Keyboard Shortcut:",
                Location = new Point(20, 325),
                Size = new Size(120, 23)
            };
            this.Controls.Add(hotkeyLabel);

            // Modifier checkboxes
            ctrlCheckBox = new CheckBox
            {
                Text = "Ctrl",
                Location = new Point(20, 350),
                Size = new Size(60, 23)
            };
            this.Controls.Add(ctrlCheckBox);

            altCheckBox = new CheckBox
            {
                Text = "Alt",
                Location = new Point(85, 350),
                Size = new Size(50, 23)
            };
            this.Controls.Add(altCheckBox);

            shiftCheckBox = new CheckBox
            {
                Text = "Shift",
                Location = new Point(140, 350),
                Size = new Size(60, 23)
            };
            this.Controls.Add(shiftCheckBox);

            winCheckBox = new CheckBox
            {
                Text = "Win",
                Location = new Point(205, 350),
                Size = new Size(50, 23)
            };
            this.Controls.Add(winCheckBox);

            // Key selection
            var keyLabel = new Label
            {
                Text = "Key:",
                Location = new Point(20, 380),
                Size = new Size(30, 23)
            };
            this.Controls.Add(keyLabel);

            keyComboBox = new ComboBox
            {
                Location = new Point(55, 380),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            PopulateKeyComboBox();
            this.Controls.Add(keyComboBox);

            // Current hotkey display
            var currentLabel = new Label
            {
                Text = "Current: Ctrl+Q",
                Location = new Point(165, 383),
                Size = new Size(150, 20),
                ForeColor = Color.Blue
            };
            currentLabel.Name = "currentLabel";
            this.Controls.Add(currentLabel);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(255, 440),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(335, 440),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            // Event handlers for updating current hotkey display
            ctrlCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            altCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            shiftCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            winCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            keyComboBox.SelectedIndexChanged += UpdateCurrentHotkeyDisplay;
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
            var commonLanguages = new Dictionary<string, string>
            {
                {"en", "English"},
                {"ru", "Русский"},
                {"es", "Español"},
                {"fr", "Français"},
                {"de", "Deutsch"},
                {"it", "Italiano"},
                {"pt", "Português"},
                {"zh", "中文"},
                {"ja", "日本語"},
                {"ko", "한국어"},
                {"ar", "العربية"},
                {"hi", "हिन्दी"},
                {"tr", "Türkçe"},
                {"pl", "Polski"},
                {"nl", "Nederlands"},
                {"sv", "Svenska"},
                {"da", "Dansk"},
                {"no", "Norsk"},
                {"fi", "Suomi"},
                {"cs", "Čeština"},
                {"uk", "Українська"},
                {"bg", "Български"},
                {"hr", "Hrvatski"},
                {"sk", "Slovenčina"},
                {"sl", "Slovenščina"},
                {"et", "Eesti"},
                {"lv", "Latviešu"},
                {"lt", "Lietuvių"},
                {"hu", "Magyar"},
                {"ro", "Română"},
                {"el", "Ελληνικά"},
                {"he", "עברית"},
                {"th", "ไทย"},
                {"vi", "Tiếng Việt"},
                {"id", "Indonesia"},
                {"ms", "Melayu"},
                {"tl", "Filipino"},
                {"fa", "فارسی"},
                {"ur", "اردو"},
                {"bn", "বাংলা"},
                {"ta", "தமிழ்"},
                {"te", "తెలుగు"},
                {"mr", "मराठी"},
                {"gu", "ગુજરાતી"},
                {"kn", "ಕನ್ನಡ"},
                {"ml", "മലയാളം"},
                {"pa", "ਪੰਜਾਬੀ"},
                {"or", "ଓଡିଆ"},
                {"as", "অসমীয়া"},
                {"ne", "नेपाली"},
                {"si", "සිංහල"},
                {"my", "မြန်မာ"},
                {"km", "ខ្មែរ"},
                {"lo", "ລາວ"},
                {"ka", "ქართული"},
                {"am", "አማርኛ"},
                {"sw", "Kiswahili"},
                {"zu", "isiZulu"},
                {"af", "Afrikaans"},
                {"is", "Íslenska"},
                {"mt", "Malti"},
                {"cy", "Cymraeg"},
                {"ga", "Gaeilge"},
                {"eu", "Euskera"},
                {"ca", "Català"},
                {"gl", "Galego"},
                {"sq", "Shqip"},
                {"mk", "Македонски"},
                {"sr", "Српски"},
                {"bs", "Bosanski"},
                {"me", "Crnogorski"},
                {"be", "Беларуская"},
                {"kk", "Қазақ"},
                {"ky", "Кыргыз"},
                {"uz", "O'zbek"},
                {"tg", "Тоҷикӣ"},
                {"mn", "Монгол"},
                {"az", "Azərbaycan"},
                {"hy", "Հայերեն"}
            };

            foreach (var lang in commonLanguages)
            {
                sourceLanguageCombo.Items.Add(new LanguageItem { Code = lang.Key, Name = lang.Value });
                targetLanguageCombo.Items.Add(new LanguageItem { Code = lang.Key, Name = lang.Value });
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
            translateModeCheckBox.Checked = Settings.UseTranslateMode;
            urlTextBox.Text = Settings.WebsiteUrl;
            
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
            
            UpdateControlVisibility();
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

        private void TranslateModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        private void UpdateControlVisibility()
        {
            bool isTranslateMode = translateModeCheckBox.Checked;
            
            // Show/hide URL controls
            urlTextBox.Enabled = !isTranslateMode;
            
            // Update test button text
            testButton.Text = isTranslateMode ? "Test Translation" : "Test Custom Website";
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (translateModeCheckBox.Checked)
                {
                    // Test translation mode
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

                    var url = $"https://translate.yandex.com/?source_lang={sourceItem.Code}&target_lang={targetItem.Code}&text={Uri.EscapeDataString(clipboardText)}";
                    
                    // Try to open in app mode
                    if (!TryOpenInAppMode(url))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) 
                        { 
                            UseShellExecute = true 
                        });
                    }
                }
                else
                {
                    // Test custom website mode
                    var url = urlTextBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        MessageBox.Show("Please enter a custom website URL to test.", "MiniTranslator", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "https://" + url;
                    }

                    // Try to open in app mode (minimal UI) with Chrome or Edge
                    if (!TryOpenInAppMode(url))
                    {
                        // Fallback to default browser if app mode fails
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) 
                        { 
                            UseShellExecute = true 
                        });
                    }
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

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Validate based on mode
            if (translateModeCheckBox.Checked)
            {
                // Validate translation settings
                if (sourceLanguageCombo.SelectedItem == null || targetLanguageCombo.SelectedItem == null)
                {
                    MessageBox.Show("Please select both source and target languages for translation mode.", 
                        "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                // Validate URL for custom website mode
                if (string.IsNullOrWhiteSpace(urlTextBox.Text))
                {
                    MessageBox.Show("Please enter a custom website URL or enable translation mode.", "MiniTranslator", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
            Settings.UseTranslateMode = translateModeCheckBox.Checked;
            Settings.WebsiteUrl = urlTextBox.Text.Trim();
            Settings.WindowWidth = widthSlider.Value;
            Settings.WindowHeight = heightSlider.Value;
            
            if (translateModeCheckBox.Checked)
            {
                var sourceItem = sourceLanguageCombo.SelectedItem as LanguageItem;
                var targetItem = targetLanguageCombo.SelectedItem as LanguageItem;
                Settings.SourceLanguage = sourceItem.Code;
                Settings.TargetLanguage = targetItem.Code;
            }
            
            Settings.HotkeyModifiers = 0;
            if (ctrlCheckBox.Checked) Settings.HotkeyModifiers |= MOD_CONTROL;
            if (altCheckBox.Checked) Settings.HotkeyModifiers |= MOD_ALT;
            if (shiftCheckBox.Checked) Settings.HotkeyModifiers |= MOD_SHIFT;
            if (winCheckBox.Checked) Settings.HotkeyModifiers |= MOD_WIN;
            
            Settings.HotkeyKey = (int)(Keys)keyComboBox.SelectedItem;
            
            // Save browser setting
            Settings.PreferredBrowser = (BrowserType)browserComboBox.SelectedIndex;
        }
    }
} 