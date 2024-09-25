@echo off
setlocal

REM Elérési útvonalak ellenőrzése
if not exist "..\..\version.txt" (
    echo version.txt file not found
    exit /b 1
)

if not exist "KoOrderRegister\KoOrderRegister.csproj" (
    echo KoOrderRegister.csproj file not found
    exit /b 1
)

set "CS_PROJECT=KoOrderRegister\KoOrderRegister.csproj"
set "OUTPUT_DIR=KoOrderRegister\bin\Release"

echo KEYPASS from environment: %KEYPASS%

echo Reading current version from version.txt...
set /p CURRENT_VERSION=<version.txt
echo Current version is %CURRENT_VERSION%

REM Verziószám kezelése
for /f "tokens=1-3 delims=." %%a in ("%CURRENT_VERSION%") do (
    set "major=%%a"
    set "minor=%%b"
    set "build=%%c"
)

set "NEW_VERSION=%major%.%minor%.%build%"
echo Updated version to %NEW_VERSION%

echo Publishing the application...
dotnet publish "%CS_PROJECT%" -f net8.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=kor.keystore -p:AndroidSigningKeyAlias=kor_pub -p:AndroidSigningKeyPass=%KEYPASS% -p:AndroidSigningStorePass=%KEYPASS%

set "ORIGINAL_APK=KoOrderRegister\bin\Release\net8.0-android\hu.kncsk.koorderregister-Signed.apk"
set "NEW_APK_NAME=KoOrderRegister\bin\Release\net8.0-android\KoOrderRegister_%NEW_VERSION%_android.apk"

echo Renaming the APK file...
if exist "%ORIGINAL_APK%" (
    move "%ORIGINAL_APK%" "%NEW_APK_NAME%"
) else (
    echo Original APK file not found
)

echo Build and rename process completed. New APK: %NEW_APK_NAME%

set "WINDOWS_NEW_VERSION=%NEW_VERSION%.0"
echo Windows version to: %WINDOWS_NEW_VERSION%

echo Building MSIX package for Windows x64...
dotnet publish "%CS_PROJECT%" -r win-x64 -c Release -f net8.0-windows10.0.19041.0 -o "%OUTPUT_DIR%\net8.0-windows-x64" -p:Version=%WINDOWS_NEW_VERSION% -p:PackageType=Msix

echo MSIX build process completed.

endlocal
