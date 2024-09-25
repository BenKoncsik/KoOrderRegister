@echo off
setlocal

if not exist "KoOrderRegister\KoOrderRegister.csproj" (
    echo KoOrderRegister.csproj file not found
    exit /b 1
)

set "CS_PROJECT=KoOrderRegister\KoOrderRegister.csproj"
REM set "OUTPUT_DIR_BUILD=\output\build"
set "OUTPUT_DIR_BUILD=bin\Release\net8.0-android"
REM set "OUTPUT_DIR=\output"
set "OUTPUT_DIR=bin\Release"

echo Create dictonary %OUTPUT_DIR_BUILD%
mkdir %OUTPUT_DIR_BUILD%
mkdir %OUTPUT_DIR%

echo KEYPASS from environment: %KEYPASS%

echo Reading current version from version.txt...

echo Version is %NEW_VERSION%
echo Version code is %NEW_VERSION_CODE%



echo Publishing the application...
dotnet publish "%CS_PROJECT%" -f net8.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=kor.keystore -p:AndroidSigningKeyAlias=kor_pub -p:AndroidSigningKeyPass=%KEYPASS% -p:AndroidSigningStorePass=%KEYPASS% -p:AndroidVersionCode=%NEW_VERSION_CODE% -p:AndroidVersionName=%NEW_VERSION%  --output "%OUTPUT_DIR_BUILD%"

set "ORIGINAL_APK=%OUTPUT_DIR_BUILD%\hu.kncsk.koorderregister-Signed.apk"
set "NEW_APK_NAME=%OUTPUT_DIR%\KoOrderRegister_%NEW_VERSION%_android.apk"

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
REM dotnet publish "%CS_PROJECT%" -r win-x64 -c Release -f net8.0-windows10.0.19041.0 --output "%OUTPUT_DIR_BUILD%" -p:Version=%WINDOWS_NEW_VERSION% -p:PackageType=Msix
REM Másolja az MSIX csomagot az általános kimeneti könyvtárba
echo Copying MSIX package to general output directory...
REM xcopy "%OUTPUT_DIR_BUILD%\*.msix" "%OUTPUT_DIR%" /Y /I
echo Copy completed.
echo MSIX build process completed.

endlocal
