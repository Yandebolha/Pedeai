# ============================================================
#  RanGoFood — Baixar pré-requisitos para o instalador
#  Execute este script UMA VEZ antes de compilar o setup.iss
#  com o Inno Setup Compiler.
#
#  O script baixa:
#    - .NET 5 Desktop Runtime x64
#    - MySQL 5.7 Community Installer (MSI)
#  e salva na pasta  installer\instaladores\
# ============================================================

$ErrorActionPreference = "Stop"
$ScriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$DestDir     = Join-Path $ScriptDir "instaladores"

if (-not (Test-Path $DestDir)) { New-Item -ItemType Directory -Path $DestDir | Out-Null }

# ── Função de download com barra de progresso ────────────────────────────────
function Download-File {
    param([string]$Url, [string]$Dest, [string]$Label)
    if (Test-Path $Dest) {
        Write-Host "  [OK] $Label — já existe, pulando download." -ForegroundColor Green
        return
    }
    Write-Host "  Baixando $Label..." -ForegroundColor Cyan
    Write-Host "  URL: $Url" -ForegroundColor DarkGray
    $wc = New-Object System.Net.WebClient
    $wc.DownloadProgressChanged += {
        $pct = $_.ProgressPercentage
        Write-Progress -Activity "Baixando $Label" -PercentComplete $pct -Status "$pct%"
    }
    $wc.DownloadFileAsync([Uri]$Url, $Dest)
    while ($wc.IsBusy) { Start-Sleep -Milliseconds 200 }
    Write-Progress -Activity "Baixando $Label" -Completed
    if (Test-Path $Dest) {
        $size = [math]::Round((Get-Item $Dest).Length / 1MB, 1)
        Write-Host "  [OK] $Label baixado ($size MB)" -ForegroundColor Green
    } else {
        Write-Host "  [ERRO] Falha ao baixar $Label" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Yellow
Write-Host "  RanGoFood — Download de Pré-Requisitos" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Yellow
Write-Host ""

# ── 1. .NET 5 Desktop Runtime x64 ───────────────────────────────────────────
# Versão 5.0.17 (última da linha 5.x)
$dotnetUrl  = "https://download.microsoft.com/download/6/b/0/6b0a4c9b-b4b7-4bc1-b9c3-f58fde5d6dc4/windowsdesktop-runtime-5.0.17-win-x64.exe"
$dotnetDest = Join-Path $DestDir "dotnet5-runtime-win-x64.exe"
Download-File -Url $dotnetUrl -Dest $dotnetDest -Label ".NET 5 Desktop Runtime x64"

# ── 2. MySQL 5.7 Community (MSI Web Installer) ──────────────────────────────
# Nota: O MySQL.com exige Accept de licença antes do download.
# O link abaixo é o instalador web (pequeno, ~2 MB) que baixa os componentes online.
# Para instalação offline, baixe manualmente o "mysql-5.7.x-winx64.msi"
# em: https://dev.mysql.com/downloads/mysql/5.7.html
$mysqlUrl  = "https://dev.mysql.com/get/Downloads/MySQLInstaller/mysql-installer-web-community-5.7.44.0.msi"
$mysqlDest = Join-Path $DestDir "mysql-installer-community.msi"

Write-Host "  Tentando baixar MySQL 5.7 Installer..." -ForegroundColor Cyan
try {
    Download-File -Url $mysqlUrl -Dest $mysqlDest -Label "MySQL 5.7 Community"
} catch {
    Write-Host ""
    Write-Host "  [AVISO] Não foi possível baixar o MySQL automaticamente." -ForegroundColor Yellow
    Write-Host "  O site do MySQL exige aceite de licença no navegador." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Faça o download manual:" -ForegroundColor White
    Write-Host "    https://dev.mysql.com/downloads/mysql/5.7.html" -ForegroundColor Cyan
    Write-Host "    (escolha 'Windows (x86, 64-bit), MSI Installer')" -ForegroundColor White
    Write-Host ""
    Write-Host "  Salve o arquivo como:" -ForegroundColor White
    Write-Host "    $mysqlDest" -ForegroundColor Cyan
    Write-Host ""
}

# ── Verifica o resultado ─────────────────────────────────────────────────────
Write-Host ""
Write-Host "=========================================" -ForegroundColor Yellow
Write-Host "  Status dos arquivos:" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Yellow

$files = @(
    @{ Path = $dotnetDest; Label = ".NET 5 Runtime" },
    @{ Path = $mysqlDest;  Label = "MySQL 5.7 Installer" }
)

$allOk = $true
foreach ($f in $files) {
    if (Test-Path $f.Path) {
        $size = [math]::Round((Get-Item $f.Path).Length / 1MB, 1)
        Write-Host "  [OK] $($f.Label) — $($f.Path) ($size MB)" -ForegroundColor Green
    } else {
        Write-Host "  [--] $($f.Label) — AUSENTE: $($f.Path)" -ForegroundColor Red
        $allOk = $false
    }
}

Write-Host ""
if ($allOk) {
    Write-Host "  Tudo pronto! Agora compile o setup.iss com o Inno Setup." -ForegroundColor Green
} else {
    Write-Host "  Há arquivos ausentes. Veja as instruções acima." -ForegroundColor Yellow
    Write-Host "  O instalador ainda funcionará — os componentes ausentes" -ForegroundColor Yellow
    Write-Host "  precisarão ser instalados manualmente pelo cliente." -ForegroundColor Yellow
}
Write-Host ""
