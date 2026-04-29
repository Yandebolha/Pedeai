; ============================================================
;  RanGoFood — Instalador para máquina do cliente
;  Ferramenta: Inno Setup 6.x  (https://jrsoftware.org/isinfo.php)
;
;  ANTES de gerar o instalador:
;    1. Execute o script `baixar_prereqs.ps1` para baixar os
;       instaladores de pré-requisitos para a pasta `instaladores\`
;    2. Publique o app:
;       dotnet publish ..\Pedeai.csproj -c Release -r win-x64 --self-contained false
;       (ou use publish self-contained se preferir)
;    3. Compile este .iss com o Inno Setup Compiler
; ============================================================

#define AppName      "RanGoFood"
#define AppVersion   "1.0"
#define AppPublisher "RanGoFood"
#define AppExe       "RanGoFood.exe"
#define PublishDir   "..\bin\Release\net5.0-windows\publish"

[Setup]
AppId={{B2A4E5C3-1F7D-4B0A-9E23-6D8C1A2F3E4B}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
; Icone do instalador (coloque o .ico na pasta installer\)
;SetupIconFile=icon.ico
OutputDir=output
OutputBaseFilename=RanGoFood_Instalador_v{#AppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
MinVersion=6.1
; Imagem lateral do wizard (opcional — 164x314 px BMP)
;WizardImageFile=wizard_side.bmp
;WizardSmallImageFile=wizard_top.bmp

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

; ── Páginas extras do wizard ─────────────────────────────────────────────────
[CustomMessages]
brazilianportuguese.MySQLPassLabel=Senha do MySQL (usuário root):
brazilianportuguese.MySQLPassDesc=Informe a senha que será definida para o usuário root do MySQL. Anote bem pois ela não pode ser recuperada depois.
brazilianportuguese.MySQLPassConfirm=Confirme a senha:
brazilianportuguese.InstallNetRuntime=Instalando .NET 5 Runtime...
brazilianportuguese.InstallMySQL=Instalando MySQL 5.7...
brazilianportuguese.ConfigDB=Configurando banco de dados...
brazilianportuguese.DotNetNotFound=O .NET 5 Runtime não foi encontrado e o arquivo de instalação não está disponível. Baixe-o em: https://dotnet.microsoft.com/download/dotnet/5.0
brazilianportuguese.MySQLNotFound=O MySQL não foi encontrado e o arquivo de instalação não está disponível. Baixe-o em: https://dev.mysql.com/downloads/mysql/

[Files]
; ── Aplicativo publicado ──────────────────────────────────────────────────────
; Certifique-se de publicar antes: dotnet publish -c Release -r win-x64
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; ── Pré-requisitos (baixados pelo script baixar_prereqs.ps1) ─────────────────
Source: "instaladores\dotnet5-runtime-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: NeedsDotNet
Source: "instaladores\mysql-installer-community.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: NeedsMySQL

[Icons]
Name: "{group}\{#AppName}";          Filename: "{app}\{#AppExe}"
Name: "{group}\Desinstalar {#AppName}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}";  Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Atalhos:"; Flags: checked

[Run]
; Instala .NET 5 Runtime se necessário
Filename: "{tmp}\dotnet5-runtime-win-x64.exe"; Parameters: "/install /quiet /norestart"; \
  StatusMsg: "{cm:InstallNetRuntime}"; Check: NeedsDotNet; Flags: waituntilterminated

; Instala MySQL se necessário (silent install com senha configurada via transform)
Filename: "msiexec.exe"; \
  Parameters: "/i ""{tmp}\mysql-installer-community.msi"" /quiet /norestart INSTALLDIR=""{pf}\MySQL\MySQL Server 5.7"" DATADIR=""{commonappdata}\MySQL\MySQL Server 5.7\Data"" MYSQL_OPENFIREWALL=1 SERVERPASSWORD=""{code:GetMySQLPass}"" SKIPSTARTMENU=1"; \
  StatusMsg: "{cm:InstallMySQL}"; Check: NeedsMySQL; Flags: waituntilterminated

; Inicia o serviço MySQL se não estiver rodando
Filename: "net"; Parameters: "start MySQL57"; Flags: runhidden waituntilterminated; Check: NeedsMySQL

; Aguarda MySQL subir e executa o banco inicial
Filename: "{sys}\cmd.exe"; \
  Parameters: "/C timeout /t 5 /nobreak >nul"; \
  Flags: runhidden waituntilterminated; Check: NeedsMySQL

[Code]

// ── Variáveis globais ─────────────────────────────────────────────────────────
var
  PageMySQLPass: TInputQueryWizardPage;
  MySQLPass: String;

// ── Detecta se .NET 5 Desktop Runtime está instalado ─────────────────────────
function IsDotNetInstalled(): Boolean;
var
  Key: String;
  Version: String;
begin
  Result := False;
  Key := 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost';
  if RegQueryStringValue(HKLM, Key, 'Version', Version) then
  begin
    // Qualquer versão 5.x serve
    if Pos('5.', Version) = 1 then
      Result := True;
  end;
  // Fallback: verifica pelo arquivo
  if not Result then
    Result := FileExists(ExpandConstant('{sys}\dotnet.exe')) or
              FileExists('C:\Program Files\dotnet\dotnet.exe');
end;

function NeedsDotNet(): Boolean;
begin
  Result := not IsDotNetInstalled();
  if Result and not FileExists(ExpandConstant('{tmp}\dotnet5-runtime-win-x64.exe')) then
  begin
    // Arquivo de instalação não disponível
    MsgBox(ExpandConstant('{cm:DotNetNotFound}'), mbError, MB_OK);
    Result := False; // não tenta instalar sem o arquivo
  end;
end;

// ── Detecta se MySQL está instalado ─────────────────────────────────────────
function IsMySQLInstalled(): Boolean;
var
  Dummy: String;
begin
  Result := False;
  // Verifica pelo serviço ou pela chave de registro
  if RegQueryStringValue(HKLM, 'SOFTWARE\MySQL AB\MySQL Server 5.7', 'Location', Dummy) then
    Result := True;
  if not Result then
    if RegQueryStringValue(HKLM, 'SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 5.7', 'Location', Dummy) then
      Result := True;
  // Verifica MySQL 8 também
  if not Result then
    if RegQueryStringValue(HKLM, 'SOFTWARE\MySQL AB\MySQL Server 8.0', 'Location', Dummy) then
      Result := True;
end;

function NeedsMySQL(): Boolean;
begin
  Result := not IsMySQLInstalled();
  if Result and not FileExists(ExpandConstant('{tmp}\mysql-installer-community.msi')) then
  begin
    MsgBox(ExpandConstant('{cm:MySQLNotFound}'), mbError, MB_OK);
    Result := False;
  end;
end;

// ── Helpers de senha ─────────────────────────────────────────────────────────
function GetMySQLPass(Param: String): String;
begin
  Result := MySQLPass;
end;

function EscapeXmlValue(S: String): String;
begin
  Result := S;
  StringChangeEx(Result, '&',  '&amp;',  True);
  StringChangeEx(Result, '<',  '&lt;',   True);
  StringChangeEx(Result, '>',  '&gt;',   True);
  StringChangeEx(Result, '"',  '&quot;', True);
  StringChangeEx(Result, '''', '&apos;', True);
end;

// ── Atualiza App.config com a senha escolhida pelo usuário ───────────────────
procedure UpdateAppConfig();
var
  ConfigPath: String;
  Content: String;
  OldConn: String;
  NewConn: String;
  EscapedPass: String;
begin
  ConfigPath := ExpandConstant('{app}\RanGoFood.exe.config');
  if not LoadStringFromFile(ConfigPath, Content) then
    Exit;

  EscapedPass := EscapeXmlValue(MySQLPass);

  // Substitui a ConnectionString preservando o resto do arquivo
  // A linha contém: value="Server=localhost;Database=pedeai;User=root;Password=XXXXX;..."
  // Usamos uma substituição simples de Password=...;
  // Encontra e substitui o valor do Password na connectionstring
  OldConn := 'Server=localhost;Database=pedeai;User=root;Password=';
  NewConn := 'Server=localhost;Database=pedeai;User=root;Password=' + EscapedPass + ';Port=3306;CharSet=utf8mb4;SslMode=None;AllowPublicKeyRetrieval=true;';

  // Substitui tudo entre ConnectionString value=" e o próximo "
  if Pos(OldConn, Content) > 0 then
  begin
    // Encontra início da connection string value
    var StartPos: Integer;
    var EndPos: Integer;
    var Marker: String;
    Marker := 'key="ConnectionString" value="';
    StartPos := Pos(Marker, Content);
    if StartPos > 0 then
    begin
      StartPos := StartPos + Length(Marker);
      EndPos := StartPos;
      while (EndPos <= Length(Content)) and (Content[EndPos] <> '"') do
        EndPos := EndPos + 1;
      Content := Copy(Content, 1, StartPos - 1) +
                 'Server=localhost;Database=pedeai;User=root;Password=' + EscapedPass +
                 ';Port=3306;CharSet=utf8mb4;SslMode=None;AllowPublicKeyRetrieval=true;' +
                 Copy(Content, EndPos, Length(Content));
    end;
  end;

  SaveStringToFile(ConfigPath, Content, False);
end;

// ── Cria banco de dados e usuário root via mysql.exe ─────────────────────────
procedure SetupDatabase();
var
  MySQLExe: String;
  SqlFile:  String;
  Cmd:      String;
  ResultCode: Integer;
begin
  // Localiza mysql.exe
  MySQLExe := 'C:\Program Files\MySQL\MySQL Server 5.7\bin\mysql.exe';
  if not FileExists(MySQLExe) then
    MySQLExe := 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe';
  if not FileExists(MySQLExe) then
    Exit; // MySQL não localizado, DbMigrator vai criar tudo no primeiro run

  // Cria o banco pedeai (o DbMigrator cria as tabelas no 1º run do app)
  SqlFile := ExpandConstant('{tmp}\create_db.sql');
  SaveStringToFile(SqlFile,
    'CREATE DATABASE IF NOT EXISTS `pedeai` DEFAULT CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci;',
    False);

  Cmd := '"' + MySQLExe + '" -u root -p"' + MySQLPass + '" < "' + SqlFile + '"';
  Exec(ExpandConstant('{sys}\cmd.exe'), '/C ' + Cmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

// ── Inicialização do wizard ───────────────────────────────────────────────────
procedure InitializeWizard();
begin
  // Página de senha do MySQL (aparece antes da instalação)
  PageMySQLPass := CreateInputQueryPage(
    wpSelectDir,
    'Configuração do Banco de Dados',
    'Defina a senha do MySQL',
    'Informe a senha que será configurada para o usuário root do MySQL. ' +
    'Esta senha será usada pelo RanGoFood para acessar o banco de dados. ' +
    'Anote-a em um lugar seguro.');

  PageMySQLPass.Add('Senha do MySQL (usuário root):', True);
  PageMySQLPass.Add('Confirme a senha:', True);

  // Valor padrão sugerido (usuário pode trocar)
  PageMySQLPass.Values[0] := 'RanGoFood@2025';
  PageMySQLPass.Values[1] := 'RanGoFood@2025';
end;

// ── Validação da página de senha ─────────────────────────────────────────────
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = PageMySQLPass.ID then
  begin
    if PageMySQLPass.Values[0] = '' then
    begin
      MsgBox('A senha não pode estar em branco.', mbError, MB_OK);
      Result := False;
      Exit;
    end;
    if PageMySQLPass.Values[0] <> PageMySQLPass.Values[1] then
    begin
      MsgBox('As senhas não coincidem. Por favor, verifique.', mbError, MB_OK);
      Result := False;
      Exit;
    end;
    if Length(PageMySQLPass.Values[0]) < 6 then
    begin
      MsgBox('A senha deve ter pelo menos 6 caracteres.', mbError, MB_OK);
      Result := False;
      Exit;
    end;
    MySQLPass := PageMySQLPass.Values[0];
  end;
end;

// ── Pós-instalação: atualiza config e cria banco ──────────────────────────────
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    UpdateAppConfig();
    SetupDatabase();
  end;
end;
