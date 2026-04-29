@echo off
chcp 65001 >nul
title RanGoFood - Publicar e Gerar Instalador

echo.
echo =========================================
echo   RanGoFood - Gerar Instalador
echo =========================================
echo.

:: Diretório raiz do projeto (um nível acima de installer\)
set ROOT=%~dp0..
set PUBLISH_DIR=%ROOT%\bin\Release\net5.0-windows\publish
set INNO=C:\Program Files (x86)\Inno Setup 6\ISCC.exe

:: ── 1. Publicar o app ────────────────────────────────────────────────────────
echo [1/3] Publicando o app...
dotnet publish "%ROOT%\Pedeai.csproj" ^
    -c Release ^
    -r win-x64 ^
    --self-contained false ^
    -p:PublishSingleFile=false ^
    -o "%PUBLISH_DIR%"

if %ERRORLEVEL% NEQ 0 (
    echo [ERRO] Falha ao publicar. Verifique se o dotnet SDK esta instalado.
    pause
    exit /b 1
)
echo [OK] App publicado em %PUBLISH_DIR%
echo.

:: ── 2. Baixar pré-requisitos (se ausentes) ───────────────────────────────────
echo [2/3] Verificando pre-requisitos...
if not exist "%~dp0instaladores\dotnet5-runtime-win-x64.exe" (
    echo Baixando pre-requisitos...
    powershell -ExecutionPolicy Bypass -File "%~dp0baixar_prereqs.ps1"
) else (
    echo [OK] Pre-requisitos ja presentes.
)
echo.

:: ── 3. Compilar o instalador ─────────────────────────────────────────────────
echo [3/3] Compilando o instalador com Inno Setup...
if not exist "%INNO%" (
    echo [AVISO] Inno Setup nao encontrado em: %INNO%
    echo Baixe em: https://jrsoftware.org/isdl.php
    echo Depois execute novamente este script.
    pause
    exit /b 1
)

"%INNO%" "%~dp0setup.iss"

if %ERRORLEVEL% NEQ 0 (
    echo [ERRO] Falha ao compilar o instalador.
    pause
    exit /b 1
)

echo.
echo =========================================
echo   SUCESSO!
echo   Instalador gerado em: installer\output\
echo =========================================
echo.
pause
