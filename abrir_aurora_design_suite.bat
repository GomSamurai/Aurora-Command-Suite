@echo off
title Aurora Design Suite Launcher
cd /d "%~dp0bin\Release\net7.0-windows"
if exist "AuroraDesignSuite.exe" (
    echo Iniciando Aurora Design Master Suite...
    start "" "AuroraDesignSuite.exe"
) else (
    echo Error: No se encontro AuroraDesignSuite.exe
    pause
)
