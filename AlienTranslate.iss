[Setup]
AppName=AlienTranslate
AppVersion=1.0.0
AppPublisher=Sefa Şenlik
AppPublisherURL=https://github.com/senliksefa/alientranslate
AppSupportURL=https://github.com/senliksefa/alientranslate/issues
AppUpdatesURL=https://github.com/senliksefa/alientranslate/releases
DefaultDirName={pf}\AlienTranslate
DefaultGroupName=AlienTranslate
UninstallDisplayIcon={app}\icon.ico
OutputDir=.
OutputBaseFilename=AlienTranslateSetup
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "docs\CHATGPT_SETUP.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "docs\CHATGPT_API_SETUP.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\AlienTranslate"; Filename: "{app}\AlienTranslate.exe"; WorkingDir: "{app}"
Name: "{group}\Uninstall AlienTranslate"; Filename: "{uninstallexe}"
Name: "{commondesktop}\AlienTranslate"; Filename: "{app}\AlienTranslate.exe"; WorkingDir: "{app}"

[Run]
Filename: "{app}\AlienTranslate.exe"; Description: "Launch AlienTranslate"; Flags: nowait postinstall skipifsilent