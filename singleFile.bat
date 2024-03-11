@echo off
setlocal enabledelayedexpansion
set "outputFile=combined_files.txt"
del "%outputFile%" 2>nul

for /r %%F in (*.cs *.json) do (
echo %%~dpF | findstr /i /c:"\bin" /c:"\obj" /c:"\packages" /c:"\properties" >nul
if errorlevel 1 (
echo File: %%F >> "%outputFile%"
type "%%F" >> "%outputFile%"
echo. >> "%outputFile%"
echo. >> "%outputFile%"
)
)