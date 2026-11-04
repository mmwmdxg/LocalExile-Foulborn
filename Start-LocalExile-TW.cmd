@echo off
setlocal
title LocalExile - Traditional Chinese
cd /d "%~dp0"
if errorlevel 1 goto failed

if not exist "scripts\dev\live.ps1" (
    echo ERROR: scripts\dev\live.ps1 was not found.
    echo Keep this launcher in the LocalExile repository root.
    goto failed
)

if not exist "artifacts\runtime-temp" mkdir "artifacts\runtime-temp"
if not exist "artifacts\runtime-temp" (
    echo ERROR: Could not create the runtime temporary directory.
    goto failed
)

set "TEMP=%CD%\artifacts\runtime-temp"
set "TMP=%TEMP%"
set "TMPDIR=%TEMP%"
rem PoeChinese3 maps the French language entry to Traditional Chinese.
set "Game_CLIENT_LANGUAGE=fr"

rem The client keeps its window and network mode only in memory, so write them before every start: windowed, 2560x1440
rem and Predictive (the only network mode this server serves). Change the numbers here, or run
rem scripts\dev\client-settings.ps1 -Width/-Height/-Fullscreen yourself.
if exist "%CD%\scripts\dev\client-settings.ps1" (
    powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%CD%\scripts\dev\client-settings.ps1" -Width 2560 -Height 1440 -NetworkMode predictive
    if errorlevel 1 echo WARNING: could not write the client settings; the game will use its own defaults.
)

echo Starting LocalExile with Traditional Chinese...
echo Do not run a second launcher while this one is starting.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%CD%\scripts\dev\live.ps1"
set "LAUNCH_EXIT=%ERRORLEVEL%"
if not "%LAUNCH_EXIT%"=="0" (
    echo.
    echo ERROR: LocalExile startup returned code %LAUNCH_EXIT%.
    pause
    exit /b %LAUNCH_EXIT%
)

echo.
echo Startup command completed. See the game window and logs for status.
pause
exit /b 0

:failed
pause
exit /b 1
