@echo off
title Aurora 4X Command Suite Launcher
color 0B
echo ============================================================
echo   INICIANDO AURORA 4X (v2.7.1) + COMMAND SUITE
echo ============================================================
echo.

if exist "%~dp0..\AuroraPatch.exe" (
    echo [1/2] Iniciando Aurora 4X con Parche (AuroraPatch.exe)...
    start "" "%~dp0..\AuroraPatch.exe"
    ping 127.0.0.1 -n 3 >nul
) else if exist "%~dp0..\Aurora.exe" (
    echo [1/2] Iniciando Aurora 4X (Aurora.exe)...
    start "" "%~dp0..\Aurora.exe"
    ping 127.0.0.1 -n 3 >nul
) else if exist "%~dp0AuroraPatch.exe" (
    echo [1/2] Iniciando Aurora 4X con Parche (AuroraPatch.exe)...
    start "" "%~dp0AuroraPatch.exe"
    ping 127.0.0.1 -n 3 >nul
) else if exist "%~dp0Aurora.exe" (
    echo [1/2] Iniciando Aurora 4X (Aurora.exe)...
    start "" "%~dp0Aurora.exe"
    ping 127.0.0.1 -n 3 >nul
) else (
    echo AuroraPatch.exe / Aurora.exe no detectado en la carpeta superior.
    echo Para interconexion total, descomprime esta carpeta dentro del directorio de Aurora 4X.
)

echo [2/2] Iniciando Aurora Command Suite...
start "" "%~dp0App\AuroraDesignSuite.exe"

echo.
echo Proceso completado. Ambos programas sincronizados.
exit
