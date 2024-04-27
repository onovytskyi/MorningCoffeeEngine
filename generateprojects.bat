@echo off

SET "SHARPMAKE_EXE=WickedEngine\Externals\Sharpmake\Sharpmake.Application\bin\Release\net6.0\Sharpmake.Application.exe" 

setlocal EnableDelayedExpansion

if not exist "%SHARPMAKE_EXE%" (
    echo Locating msbuild.exe
    set VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
    for /f "tokens=1 delims=;" %%i in ('"!VSWHERE!" -nologo -latest -property installationPath') do SET "MSBUILDROOT=%%i"
    for /f "tokens=1" %%i in ('"!VSWHERE!" -property installationVersion') do SET "MSBUILDVER=%%i"
    set MSBUILDPATH="!MSBUILDROOT!\MSBuild\!MSBUILDVER:~0,2!.0\Bin\MSBuild.exe"
    if not exist "!MSBUILDPATH!" set MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\amd64\MSBuild.exe"
    if not exist "!MSBUILDPATH!" set MSBUILDPATH="!MSBUILDROOT!\MSBuild\Current\Bin\MSBuild.exe"
    echo Found MSBuild at !MSBUILDPATH!

    echo Compiling Sharpmake
    !MSBUILDPATH! WickedEngine\Externals\Sharpmake\Sharpmake.sln /p:Configuration=Release /p:Platform="Any CPU"
) 

!SHARPMAKE_EXE! /sources('./Projects/MorningCoffee.sharpmake.cs')