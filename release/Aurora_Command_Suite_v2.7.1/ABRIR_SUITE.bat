@echo off
title Aurora 4X Command Suite Launcher
color 0B
pushd "%~dp0"

echo ============================================================
echo   INICIANDO AURORA 4X (v2.7.1) + COMMAND SUITE
echo ============================================================
echo.

if exist "App\Patches\AuroraSpanish" (
    if exist "..\Patches" xcopy /E /Y /Q "App\Patches\AuroraSpanish" "..\Patches\AuroraSpanish\" >nul 2>&1
    if exist "Patches" xcopy /E /Y /Q "App\Patches\AuroraSpanish" "Patches\AuroraSpanish\" >nul 2>&1
)

tasklist /fi "IMAGENAME eq Aurora.exe" 2>NUL | find /I /N "Aurora.exe" >NUL
if "%ERRORLEVEL%"=="0" (
    echo [INFO] Aurora 4X ya se encuentra ejecutandose.
    goto LAUNCH_SUITE
)

tasklist /fi "IMAGENAME eq AuroraPatch.exe" 2>NUL | find /I /N "AuroraPatch.exe" >NUL
if "%ERRORLEVEL%"=="0" (
    echo [INFO] AuroraPatch.exe ya se encuentra ejecutandose.
    goto LAUNCH_SUITE
)

if exist "..\AuroraPatch.exe" goto LAUNCH_PATCH_PARENT
if exist "..\Aurora.exe" goto LAUNCH_GAME_PARENT
if exist "AuroraPatch.exe" goto LAUNCH_PATCH_LOCAL
if exist "Aurora.exe" goto LAUNCH_GAME_LOCAL

echo [AVISO] AuroraPatch.exe / Aurora.exe no detectado en la carpeta del juego.
echo Se iniciara unicamente la Suite de Mando...
echo.
goto LAUNCH_SUITE

:LAUNCH_PATCH_PARENT
echo [1/2] Iniciando Aurora 4X con Parche (AuroraPatch.exe)...
start /d "%~dp0.." "" "%~dp0..\AuroraPatch.exe"
ping 127.0.0.1 -n 3 >nul
goto LAUNCH_SUITE

:LAUNCH_GAME_PARENT
echo [1/2] Iniciando Aurora 4X (Aurora.exe)...
start /d "%~dp0.." "" "%~dp0..\Aurora.exe"
ping 127.0.0.1 -n 3 >nul
goto LAUNCH_SUITE

:LAUNCH_PATCH_LOCAL
echo [1/2] Iniciando Aurora 4X con Parche (AuroraPatch.exe)...
start /d "%~dp0" "" "%~dp0AuroraPatch.exe"
ping 127.0.0.1 -n 3 >nul
goto LAUNCH_SUITE

:LAUNCH_GAME_LOCAL
echo [1/2] Iniciando Aurora 4X (Aurora.exe)...
start /d "%~dp0" "" "%~dp0Aurora.exe"
ping 127.0.0.1 -n 3 >nul
goto LAUNCH_SUITE

:LAUNCH_SUITE
if exist "App\AuroraDesignSuite.exe" (
    echo [2/2] Iniciando Aurora Command Suite...
    start /d "%~dp0App" "" "%~dp0App\AuroraDesignSuite.exe"
    echo.
    echo [EXITO] Ambos programas se estan ejecutando en paralelo.
    ping 127.0.0.1 -n 2 >nul
    popd
    exit /b 0
)

if exist "AuroraDesignSuite.exe" (
    echo [2/2] Iniciando Aurora Command Suite...
    start /d "%~dp0" "" "%~dp0AuroraDesignSuite.exe"
    echo.
    echo [EXITO] Ambos programas se estan ejecutando en paralelo.
    ping 127.0.0.1 -n 2 >nul
    popd
    exit /b 0
)

color 0C
echo [ERROR] No se pudo encontrar App\AuroraDesignSuite.exe.
echo Asegurate de no haber movido o borrado la carpeta App.
pause
popd
exit /b 1
