@echo off
:: Instalar PedeaiUpdateService como serviço Windows
:: Execute este script como Administrador

set SVC_NAME=PedeaiUpdateService
set SVC_EXE=%~dp0PedeaiUpdateService.exe
set SVC_DESC=Serviço de Atualização Automática RanGoFood/Pedeai

echo Instalando %SVC_NAME%...

sc stop "%SVC_NAME%" 2>nul
sc delete "%SVC_NAME%" 2>nul
timeout /t 2 /nobreak >nul

sc create "%SVC_NAME%" binPath= "\"%SVC_EXE%\"" start= auto DisplayName= "Pedeai Update Service"
sc description "%SVC_NAME%" "%SVC_DESC%"
sc start "%SVC_NAME%"

if %ERRORLEVEL% == 0 (
    echo Servico instalado e iniciado com sucesso!
) else (
    echo ERRO ao instalar servico. Execute como Administrador.
)
pause
