@echo off
echo.
echo ========================================
echo    Starting EchoBridge Audio Mixer
echo ========================================
echo.
cd /d "%~dp0bin\Release\net8.0-windows\win-x64\publish"
start EchoBridge.exe
echo EchoBridge is launching...
echo.
timeout /t 2 >nul
