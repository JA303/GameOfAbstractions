@echo off
setlocal
title Game of Abstractions
rem Use the same per-user .NET installation as the IDE, if available.
set "host=dotnet.exe"
if exist "%USERPROFILE%\.dotnet\dotnet.exe" set "host=%USERPROFILE%\.dotnet\dotnet.exe"
if not exist "%~dp0Play\Game.dll" (
    echo Game not found. Extract the entire ZIP and keep Play beside this file.
    set "result=1"
    goto finish
)
"%host%" "%~dp0Play\Game.dll"
set "result=%errorlevel%"
if not "%result%"=="0" echo Game exited with code %result%. See the error above.
:finish
echo.
pause
exit /b %result%
