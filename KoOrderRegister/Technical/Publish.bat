@echo off
setlocal

REM A script aktuális mappájából indítva
set "CS_PROJECT=..\KoOrderRegister.csproj"
set "OUTPUT_DIR=..\bin\Release"

echo Backing up the project file...
copy "%CS_PROJECT%" "Technical\%CS_PROJECT%.bak"

echo Reading current version...
for /f "tokens=3 delims=<>" %%a in ('findstr "ApplicationDisplayVersion" %CS_PROJECT%') do set "CURRENT_VERSION=%%a"
echo Current version is %CURRENT_VERSION%

REM Bontsa szét a verziót pontok mentén
for /f "tokens=1-3 delims=." %%a in ("%CURRENT_VERSION%") do (
    set "major=%%a"
    set "minor=%%b"
    set "build=%%c"
    REM set "revision=%%d"
)

REM Növelje a harmadik verziószámot
set /a "build+=1"

REM Új verzió összeállítása
set "NEW_VERSION=%major%.%minor%.%build%"
echo Updated version to %NEW_VERSION%

echo Updating the project file with new version...
REM powershell -Command "(gc '%CS_PROJECT%') -replace '<ApplicationDisplayVersion>%CURRENT_VERSION%</ApplicationDisplayVersion>', '<ApplicationDisplayVersion>%NEW_VERSION%</ApplicationDisplayVersion>' | Out-File -encoding UTF8 '%CS_PROJECT%'"

echo Please enter your keystore password:
set /p KEYPASS=""

echo sett icons
set "ICON_SOURCE_DIR=images\Dev\icons"
set "ICON_TARGET_DIR=..\Resources\Appicon"

set "SPLASH_SOURCE_DIR=images\Dev\splash"
set "SPLASH_TARGET_DIR=..\Resources\Splash"

REM Másolja a fejlesztői ikonokat a célmappába
xcopy /Y "%ICON_SOURCE_DIR%\*.*" "%ICON_TARGET_DIR%"
xcopy /Y "%SPLASH_SOURCE_DIR%\*.*" "%SPLASH_TARGET_DIR%"

echo Publishing the application...
dotnet publish "..\KoOrderRegister.csproj" -f net8.0-android -c DevBuild -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=kor.keystore -p:AndroidSigningKeyAlias=kor_pub -p:AndroidSigningKeyPass=%KEYPASS% -p:AndroidSigningStorePass=%KEYPASS%

REM echo Delete key store password
REM set "KeystorePassword="

REM Cél APK útvonal
set "ORIGINAL_APK=..\bin\Release\net8.0-android\hu.kncsk.koorderregister-Signed.apk"

REM Új APK neve a frissített verzióval
set "NEW_APK_NAME=..\bin\Release\net8.0-android\KoOrderRegister_%NEW_VERSION%_android.apk"



echo Renaming the APK file...
REM move "%ORIGINAL_APK%" "%NEW_APK_NAME%"

echo Build and rename process completed. New APK: %NEW_APK_NAME%

set "WINDOWS_NEW_VERSION=%NEW_VERSION%.0"
echo Windows version to: %WINDOWS_NEW_VERSION%

echo Building MSIX package for Windows x64...
REM sdotnet publish "%CS_PROJECT%" -r win-x64 -c Release -f net8.0-windows10.0.19041.0 -o "%OUTPUT_DIR%\net8.0-windows-x64" -p:Version=%WINDOWS_NEW_VERSION% -p:PackageType=Msix



echo Building MSIX package for Windows ARM64...
REM dotnet publish "%CS_PROJECT%" -r win-arm64 -c Release -f net8.0-windows10.0.19041.0 -p:PublishSingleFile=true -o "%OUTPUT_DIR%\net8.0-windows-arm64" -p:Version=%NEW_VERSION% -p:PackageType=Msix
REM dotnet publish "%CS_PROJECT%" -r win-arm64 -c Release -f net8.0-windows10.0.19041.0 -o "%OUTPUT_DIR%\net8.0-windows-arm64" -p:Version=%WINDOWS_NEW_VERSION% -p:PackageType=Msix

echo MSIX build process completed.


endlocal


