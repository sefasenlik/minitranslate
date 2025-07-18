using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace MiniTranslate
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
        private Label widthLabel;
        private Label heightLabel;
        private ComboBox browserComboBox;
        private Button okButton;
        private Button cancelButton;
        private Button testButton;
        private CheckBox startupCheckBox;
        private TextBox chatGptApiKeyTextBox;
        private Label chatGptApiKeyLabel;
        private TextBox translationServerUrlTextBox;
        private Label translationServerUrlLabel;
        private TextBox translationServerTokenTextBox;
        private Label translationServerTokenLabel;
        private GroupBox apiGroupBox;
        private GroupBox languageGroupBox;
        private GroupBox hotkeyGroupBox;
        private GroupBox windowSizeGroupBox;
        private CheckBox autoSwitchLanguagesCheckBox;
        private NumericUpDown widthNumeric;
        private NumericUpDown heightNumeric;
        // Removed hotkey type selection - now determined automatically based on key combination

        // Hotkey modifiers
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        private const string AppName = "MiniTranslate";
        private static readonly RegistryKey StartupRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public SettingsForm(AppSettings currentSettings)
        {
            Settings = new AppSettings
            {
                HotkeyModifiers = currentSettings.HotkeyModifiers,
                HotkeyKey = currentSettings.HotkeyKey,
                HotkeyType = currentSettings.HotkeyType,
                SourceLanguage = currentSettings.SourceLanguage,
                TargetLanguage = currentSettings.TargetLanguage,
                PreferredTranslator = currentSettings.PreferredTranslator,
                WindowWidth = currentSettings.WindowWidth,
                WindowHeight = currentSettings.WindowHeight,
                PreferredBrowser = currentSettings.PreferredBrowser,
                ChatGptApiKey = currentSettings.ChatGptApiKey,
                TranslationServerUrl = currentSettings.TranslationServerUrl,
                TranslationServerToken = currentSettings.TranslationServerToken,
                AutoSwitchLanguages = currentSettings.AutoSwitchLanguages
            };

            InitializeComponent();
            LoadCurrentSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "MiniTranslate Settings";
            this.Size = new Size(520, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(10);

            // Translator selection
            var translatorLabel = new Label
            {
                Text = "Translation Service:",
                Location = new Point(20, 17),
                Size = new Size(130, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            this.Controls.Add(translatorLabel);

            translatorComboBox = new ComboBox
            {
                Location = new Point(155, 15),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            translatorComboBox.Items.Add("Yandex Translate");
            translatorComboBox.Items.Add("Google Translate");
            translatorComboBox.Items.Add("ChatGPT Translator");
            translatorComboBox.Items.Add("Translation Server");
            translatorComboBox.SelectedIndexChanged += TranslatorComboBox_SelectedIndexChanged;
            this.Controls.Add(translatorComboBox);

            // API Group
            apiGroupBox = new GroupBox
            {
                Text = "API Settings",
                Location = new Point(10, 50),
                Size = new Size(480, 135)
            };
            this.Controls.Add(apiGroupBox);

            chatGptApiKeyLabel = new Label
            {
                Text = "ChatGPT API Key:",
                Location = new Point(15, 27),
                Size = new Size(120, 23)
            };
            apiGroupBox.Controls.Add(chatGptApiKeyLabel);

            chatGptApiKeyTextBox = new TextBox
            {
                Location = new Point(140, 25),
                Size = new Size(310, 23),
                UseSystemPasswordChar = true
            };
            apiGroupBox.Controls.Add(chatGptApiKeyTextBox);

            translationServerUrlLabel = new Label
            {
                Text = "Translation Server:",
                Location = new Point(15, 72),
                Size = new Size(120, 23)
            };
            apiGroupBox.Controls.Add(translationServerUrlLabel);

            translationServerUrlTextBox = new TextBox
            {
                Location = new Point(140, 70),
                Size = new Size(310, 23)
            };
            apiGroupBox.Controls.Add(translationServerUrlTextBox);

            translationServerTokenLabel = new Label
            {
                Text = "Server Token:",
                Location = new Point(15, 97),
                Size = new Size(120, 23)
            };
            apiGroupBox.Controls.Add(translationServerTokenLabel);

            translationServerTokenTextBox = new TextBox
            {
                Location = new Point(140, 95),
                Size = new Size(310, 23),
                UseSystemPasswordChar = true
            };
            apiGroupBox.Controls.Add(translationServerTokenTextBox);

            // Language Group
            languageGroupBox = new GroupBox
            {
                Text = "Languages",
                Location = new Point(10, 195),
                Size = new Size(480, 100)
            };
            this.Controls.Add(languageGroupBox);

            var sourceLabel = new Label
            {
                Text = "Source Language:",
                Location = new Point(15, 32),
                Size = new Size(110, 23)
            };
            languageGroupBox.Controls.Add(sourceLabel);

            sourceLanguageCombo = new ComboBox
            {
                Location = new Point(125, 30),
                Size = new Size(110, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            languageGroupBox.Controls.Add(sourceLanguageCombo);

            var targetLabel = new Label
            {
                Text = "Target Language:",
                Location = new Point(260, 32),
                Size = new Size(105, 23)
            };
            languageGroupBox.Controls.Add(targetLabel);

            targetLanguageCombo = new ComboBox
            {
                Location = new Point(365, 30),
                Size = new Size(95, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            languageGroupBox.Controls.Add(targetLanguageCombo);

            // Populate language combo boxes after they're created and added
            PopulateLanguageComboBoxes();
            
            // Add event handler for source language selection change
            sourceLanguageCombo.SelectedIndexChanged += SourceLanguageCombo_SelectedIndexChanged;

            // Auto-switch languages checkbox
            autoSwitchLanguagesCheckBox = new CheckBox
            {
                Text = "Auto-switch languages based on detected text script",
                Location = new Point(15, 65),
                Size = new Size(400, 23),
                Checked = true
            };
            languageGroupBox.Controls.Add(autoSwitchLanguagesCheckBox);

            // Hotkey Group
            hotkeyGroupBox = new GroupBox
            {
                Text = "Global Hotkey",
                Location = new Point(10, 305),
                Size = new Size(480, 90)
            };
            this.Controls.Add(hotkeyGroupBox);

            ctrlCheckBox = new CheckBox
            {
                Text = "Ctrl",
                Location = new Point(15, 30),
                Size = new Size(50, 23)
            };
            ctrlCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            hotkeyGroupBox.Controls.Add(ctrlCheckBox);

            altCheckBox = new CheckBox
            {
                Text = "Alt",
                Location = new Point(70, 30),
                Size = new Size(45, 23)
            };
            altCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            hotkeyGroupBox.Controls.Add(altCheckBox);

            shiftCheckBox = new CheckBox
            {
                Text = "Shift",
                Location = new Point(125, 30),
                Size = new Size(50, 23)
            };
            shiftCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            hotkeyGroupBox.Controls.Add(shiftCheckBox);

            winCheckBox = new CheckBox
            {
                Text = "Win",
                Location = new Point(180, 30),
                Size = new Size(55, 23)
            };
            winCheckBox.CheckedChanged += UpdateCurrentHotkeyDisplay;
            hotkeyGroupBox.Controls.Add(winCheckBox);

            // Key selection (inside hotkey group)
            var keyLabel = new Label
            {
                Text = "Key:",
                Location = new Point(240, 32),
                Size = new Size(30, 23)
            };
            hotkeyGroupBox.Controls.Add(keyLabel);

            keyComboBox = new ComboBox
            {
                Location = new Point(275, 30),
                Size = new Size(80, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            keyComboBox.SelectedIndexChanged += UpdateCurrentHotkeyDisplay;
            hotkeyGroupBox.Controls.Add(keyComboBox);

            PopulateKeyComboBox();

            // Current hotkey display (inside hotkey group)
            var currentLabel = new Label
            {
                Text = "Current: Ctrl+Q",
                Location = new Point(15, 62),
                Size = new Size(200, 20),
                ForeColor = Color.Blue
            };
            currentLabel.Name = "currentLabel";
            hotkeyGroupBox.Controls.Add(currentLabel);

            // Browser Settings Group
            windowSizeGroupBox = new GroupBox
            {
                Text = "Browser Settings",
                Location = new Point(10, 405),
                Size = new Size(480, 110)
            };
            this.Controls.Add(windowSizeGroupBox);

            var browserLabel = new Label
            {
                Text = "Preferred Browser:",
                Location = new Point(15, 32),
                Size = new Size(120, 23),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            windowSizeGroupBox.Controls.Add(browserLabel);

            browserComboBox = new ComboBox
            {
                Location = new Point(135, 30),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            browserComboBox.Items.Add("Chrome");
            browserComboBox.Items.Add("Edge");
            browserComboBox.Items.Add("Default Browser");
            windowSizeGroupBox.Controls.Add(browserComboBox);

            widthLabel = new Label
            {
                Text = "Window Width (px):",
                Location = new Point(15, 72),
                Size = new Size(115, 23)
            };
            windowSizeGroupBox.Controls.Add(widthLabel);

            var maxWidth = Screen.PrimaryScreen?.Bounds.Width ?? 1600;
            var maxHeight = Screen.PrimaryScreen?.Bounds.Height ?? 1060;

            widthNumeric = new NumericUpDown
            {
                Location = new Point(135, 70),
                Size = new Size(60, 23),
                Minimum = 470,
                Maximum = maxWidth,
                Value = Math.Max(470, Math.Min(Settings.WindowWidth, maxWidth)),
                Increment = 10,
                TextAlign = HorizontalAlignment.Right
            };
            windowSizeGroupBox.Controls.Add(widthNumeric);

            heightLabel = new Label
            {
                Text = "Height (px):",
                Location = new Point(220, 72),
                Size = new Size(80, 23)
            };
            windowSizeGroupBox.Controls.Add(heightLabel);

            heightNumeric = new NumericUpDown
            {
                Location = new Point(300, 70),
                Size = new Size(60, 23),
                Minimum = 350,
                Maximum = maxHeight,
                Value = Math.Max(350, Math.Min(Settings.WindowHeight, maxHeight)),
                Increment = 10,
                TextAlign = HorizontalAlignment.Right
            };
            windowSizeGroupBox.Controls.Add(heightNumeric);

            // Startup Checkbox
            startupCheckBox = new CheckBox
            {
                Text = "Run at Windows startup",
                Location = new Point(20, 530),
                Size = new Size(180, 23)
            };
            this.Controls.Add(startupCheckBox);

            // Test button
            testButton = new Button
            {
                Text = "Test Translation",
                Location = new Point(20, 565),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(225, 240, 255),
                FlatStyle = FlatStyle.Flat,
            };
            testButton.FlatAppearance.BorderColor = Color.FromArgb(173, 216, 230);
            testButton.FlatAppearance.BorderSize = 1;
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(330, 565),
                Size = new Size(75, 30)
            };
            okButton.DialogResult = DialogResult.OK;
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(415, 565),
                Size = new Size(75, 30)
            };
            cancelButton.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

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
                if (key == Keys.C)
                {
                    keyComboBox.Items.Add("C (Double)");
                }
                else
                {
                    keyComboBox.Items.Add(key);
                }
            }
        }

        private void PopulateLanguageComboBoxes()
        {
            var languages = LanguageProvider.GetLanguages();
            foreach (var lang in languages)
            {
                var item = new LanguageItem { Code = lang.Key, Name = lang.Value };
                sourceLanguageCombo.Items.Add(item);
                
                // Exclude "auto" from target language list
                if (lang.Key != "auto")
                {
                    targetLanguageCombo.Items.Add(item);
                }
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
            // Enable/disable API key field
            bool isChatGpt = Settings.PreferredTranslator == TranslatorType.ChatGPT;
            chatGptApiKeyTextBox.Enabled = isChatGpt;
            chatGptApiKeyLabel.Enabled = isChatGpt;
            // Load API key
            chatGptApiKeyTextBox.Text = Settings.ChatGptApiKey;
            translationServerUrlTextBox.Text = Settings.TranslationServerUrl;
            translationServerTokenTextBox.Text = Settings.TranslationServerToken;
            
            widthNumeric.Value = Math.Max(widthNumeric.Minimum, Math.Min(widthNumeric.Maximum, Settings.WindowWidth));
            heightNumeric.Value = Math.Max(heightNumeric.Minimum, Math.Min(heightNumeric.Maximum, Settings.WindowHeight));
            
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
            
            // Handle the special case for 'C' key
            if (Settings.HotkeyKey == (int)Keys.C)
            {
                keyComboBox.SelectedItem = "C (Double)";
            }
            else
            {
                keyComboBox.SelectedItem = (Keys)Settings.HotkeyKey;
            }
            
            // Load browser setting
            browserComboBox.SelectedIndex = (int)Settings.PreferredBrowser;
            
            // Load auto-switch languages setting
            autoSwitchLanguagesCheckBox.Checked = Settings.AutoSwitchLanguages;
            
            // Update auto-switch checkbox state based on source language
            UpdateAutoSwitchCheckBoxState();
            
            UpdateCurrentHotkeyDisplay(null, null);
        }

        private void UpdateCurrentHotkeyDisplay(object sender, EventArgs e)
        {
            var currentLabel = hotkeyGroupBox.Controls.Find("currentLabel", false).FirstOrDefault() as Label;
            if (currentLabel != null)
            {
                var parts = new System.Collections.Generic.List<string>();
                
                if (ctrlCheckBox.Checked) parts.Add("Ctrl");
                if (altCheckBox.Checked) parts.Add("Alt");
                if (shiftCheckBox.Checked) parts.Add("Shift");
                if (winCheckBox.Checked) parts.Add("Win");
                
                if (keyComboBox.SelectedItem != null)
                {
                    var keyText = keyComboBox.SelectedItem.ToString();
                    if (keyText == "C (Double)")
                    {
                        parts.Add("C");
                    }
                    else
                    {
                        parts.Add(keyText);
                    }
                }
                
                var hotkeyString = parts.Count > 0 ? string.Join("+", parts) : "(none)";
                
                // Add double press indicator for CTRL+C
                if (keyComboBox.SelectedItem != null && keyComboBox.SelectedItem.ToString() == "C (Double)" && ctrlCheckBox.Checked)
                {
                    hotkeyString += "+C";
                }
                
                currentLabel.Text = $"Current: {hotkeyString}";
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
                        "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var sourceItem = sourceLanguageCombo.SelectedItem as LanguageItem;
                var targetItem = targetLanguageCombo.SelectedItem as LanguageItem;
                
                if (sourceItem == null || targetItem == null)
                {
                    MessageBox.Show("Please select both source and target languages.", 
                        "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"Failed to open: {ex.Message}", "MiniTranslate", 
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
                    "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (translatorComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a translation service.", 
                    "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate hotkey
            if (keyComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a key for the hotkey.", "MiniTranslate", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ctrlCheckBox.Checked && !altCheckBox.Checked && !shiftCheckBox.Checked && !winCheckBox.Checked)
            {
                MessageBox.Show("Please select at least one modifier key (Ctrl, Alt, Shift, or Win).", 
                    "MiniTranslate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // hotkeyTypeComboBox.SelectedIndex = -1; // Removed hotkey type selection

            // Save settings
            Settings.PreferredTranslator = (TranslatorType)translatorComboBox.SelectedIndex;
            Settings.WindowWidth = (int)widthNumeric.Value;
            Settings.WindowHeight = (int)heightNumeric.Value;
            
            var sourceItem = sourceLanguageCombo.SelectedItem as LanguageItem;
            var targetItem = targetLanguageCombo.SelectedItem as LanguageItem;
            Settings.SourceLanguage = sourceItem.Code;
            Settings.TargetLanguage = targetItem.Code;
            
            Settings.HotkeyModifiers = 0;
            if (ctrlCheckBox.Checked) Settings.HotkeyModifiers |= MOD_CONTROL;
            if (altCheckBox.Checked) Settings.HotkeyModifiers |= MOD_ALT;
            if (shiftCheckBox.Checked) Settings.HotkeyModifiers |= MOD_SHIFT;
            if (winCheckBox.Checked) Settings.HotkeyModifiers |= MOD_WIN;
            
            // Handle the special case for 'C (Double)' key
            if (keyComboBox.SelectedItem.ToString() == "C (Double)")
            {
                Settings.HotkeyKey = (int)Keys.C;
                Settings.HotkeyType = HotkeyType.DoubleKey;
            }
            else
            {
                Settings.HotkeyKey = (int)(Keys)keyComboBox.SelectedItem;
                Settings.HotkeyType = HotkeyType.SingleKey;
            }
            
            // Save browser setting
            Settings.PreferredBrowser = (BrowserType)browserComboBox.SelectedIndex;

            // Save API key
            Settings.ChatGptApiKey = chatGptApiKeyTextBox.Text.Trim();
            Settings.TranslationServerUrl = translationServerUrlTextBox.Text.Trim();
            Settings.TranslationServerToken = translationServerTokenTextBox.Text.Trim();

            // Save auto-switch languages setting
            Settings.AutoSwitchLanguages = autoSwitchLanguagesCheckBox.Checked;

            // Handle Startup setting
            SetStartup(startupCheckBox.Checked);
        }

        private void TranslatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTranslatorFields();
        }

        private void UpdateTranslatorFields()
        {
            bool isChatGpt = translatorComboBox.SelectedIndex == 2;
            bool isTranslationServer = translatorComboBox.SelectedIndex == 3;
            
            // ChatGPT fields
            chatGptApiKeyTextBox.Enabled = isChatGpt;
            chatGptApiKeyLabel.Enabled = isChatGpt;
            
            // Translation Server fields
            translationServerUrlLabel.Enabled = isTranslationServer;
            translationServerUrlTextBox.Enabled = isTranslationServer;
            translationServerTokenLabel.Enabled = isTranslationServer;
            translationServerTokenTextBox.Enabled = isTranslationServer;
        }
        
        private void SourceLanguageCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAutoSwitchCheckBoxState();
        }
        
        private void UpdateAutoSwitchCheckBoxState()
        {
            var selectedItem = sourceLanguageCombo.SelectedItem as LanguageItem;
            bool isAutoLanguage = selectedItem?.Code == "auto";
            
            if (isAutoLanguage)
            {
                autoSwitchLanguagesCheckBox.Enabled = false;
                autoSwitchLanguagesCheckBox.Checked = false;
                autoSwitchLanguagesCheckBox.Text = "Auto-switch languages (disabled when 'Detect language' is selected)";
            }
            else
            {
                autoSwitchLanguagesCheckBox.Enabled = true;
                autoSwitchLanguagesCheckBox.Text = "Auto-switch languages based on detected text script";
            }
        }
    }
} 