@echo off
title Desinstalador - Aurora Command Suite
color 0C
echo ============================================================
echo   DESINSTALADOR OFICIAL DE AURORA COMMAND SUITE
echo ============================================================
echo.
echo Esta accion borrara los archivos de la Suite manteniendo tu partida
echo e instalacion de Aurora 4X totalmente intactas.
echo.
set /p CONFIRM="¿Estas seguro de que deseas desinstalar la suite? (S/N): "
if /i "%CONFIRM%" NEQ "S" goto CANCEL

echo.
echo [1/2] Cerrando procesos activos de la suite...
taskkill /f /im AuroraDesignSuite.exe >nul 2>&1

echo [2/2] Eliminando carpeta App y componentes...
if exist "%~dp0App" rmdir /s /q "%~dp0App" >nul 2>&1

echo.
echo ============================================================
echo   DESINSTALACION COMPLETADA CON EXITO
echo   Tu juego Aurora 4X y tu base de datos siguen intactos.
echo ============================================================
pause
exit

:CANCEL
echo.
echo Desinstalacion cancelada por el usuario.
pause
exit
