@echo off
echo Pulisco cache...
dotnet clean
echo.
echo Compilazione...
dotnet build
echo.
echo Fatto.
pause
