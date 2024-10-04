@echo off
setlocal



set "CS_PROJECT=KoOrderRegister\KoOrderRegister.csproj"
set "APPX_MANIFEST=KoOrderRegister\Platforms\Windows\Package.appxmanifest"
set "OUTPUT_DIR_BUILD=output\build"
set "OUTPUT_DIR=output"

if not exist %CS_PROJECT% (
    echo %CS_PROJECT% file not found
    exit /b 1
)

if not exist %APPX_MANIFEST% (
    echo %APPX_MANIFEST% file not found
    exit /b 1
)

echo Build version: %BUILD_VERSION%
if "%BUILD_VERSION%"=="DEV_VERSION" (
    set publish_version=Debug
) else (
    set publish_version=Release
)
echo publish_version: %publish_version%

echo Reading current version...
for /f "tokens=3 delims=<>" %%a in ('findstr "ApplicationDisplayVersion" %CS_PROJECT%') do set "CURRENT_VERSION=%%a"
set "WINDOWS_CURRENT_VERSION=%CURRENT_VERSION%.0"
echo Current version is %CURRENT_VERSION%
echo Current windows version is %WINDOWS_CURRENT_VERSION%

REM Bontsa szét a verziót pontok mentén
for /f "tokens=1-3 delims=." %%a in ("%CURRENT_VERSION%") do (
    set "major=%%a"
    set "minor=%%b"
    set "build=%%c"
)

echo Updated version to %NEW_VERSION%

set "WINDOWS_NEW_VERSION=%NEW_VERSION%.0"
echo Windows version to: %WINDOWS_NEW_VERSION%

echo Updating the project file with new version...
REM android
powershell -Command "(gc '%CS_PROJECT%') -replace '<ApplicationDisplayVersion>%CURRENT_VERSION%</ApplicationDisplayVersion>', '<ApplicationDisplayVersion>%NEW_VERSION%</ApplicationDisplayVersion>' | Out-File -encoding UTF8 '%CS_PROJECT%'"

REM windows
powershell -Command "(gc '%APPX_MANIFEST%') -replace 'Version=\"%WINDOWS_CURRENT_VERSION%\"', 'Version=\"%WINDOWS_NEW_VERSION%\"' | Out-File -encoding UTF8 '%APPX_MANIFEST%'"



echo Create dictonary %OUTPUT_DIR_BUILD%
mkdir %OUTPUT_DIR_BUILD%
mkdir %OUTPUT_DIR%

echo KEYPASS from environment: %KEYPASS%

echo Reading current version from version.txt...

echo Version is %NEW_VERSION%
echo Version code is %NEW_VERSION_CODE%



echo Publishing the application...
dotnet publish "%CS_PROJECT%" -f net8.0-android -c %publish_version% -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=kor.keystore -p:AndroidSigningKeyAlias=kor_pub -p:AndroidSigningKeyPass=%KEYPASS% -p:AndroidSigningStorePass=%KEYPASS% -p:AndroidVersionCode=%NEW_VERSION_CODE% -p:AndroidVersionName=%NEW_VERSION%  --output "%OUTPUT_DIR_BUILD%"

if "%BUILD_VERSION%"=="DEV_VERSION" (
	set "ORIGINAL_APK=%OUTPUT_DIR_BUILD%\hu.kncsk.debug.koorderregister-Signed.apk"
) else (
	set "ORIGINAL_APK=%OUTPUT_DIR_BUILD%\hu.kncsk.koorderregister-Signed.apk"
)

echo Apk path: %ORIGINAL_APK%

set "NEW_APK_NAME=%OUTPUT_DIR%\KoOrderRegister_%NEW_VERSION%_android_%BUILD_VERSION%.apk"

echo Renaming the APK file...
if exist "%ORIGINAL_APK%" (
    move "%ORIGINAL_APK%" "%NEW_APK_NAME%"
) else (
    echo Original APK file not found
)

echo Build and rename process completed. New APK: %NEW_APK_NAME%




echo Building MSIX package for Windows x64...
REM dotnet publish ".\KoOrderRegister\KoOrderRegister.csproj" -r win-x64 -c Release -f net8.0-windows10.0.19041.0 --output "output/build/" -p:PackageType=Msix -p:PackageCertificateKeyFile="Technical\kor.pfx" -p:PackageCertificatePassword="kor" -p:PackageCertificateThumbprint="52E6E26AD745DE7F7EB2CDC031509D57F78CEBF8" -v diag -p:AppxPackageDir="../output/"

dotnet publish ".\KoOrderRegister\KoOrderRegister.csproj" ^
  -r win-x64 -c %publish_version% -f net8.0-windows10.0.19041.0 ^
  --output "output/build/" ^
  -p:PackageType=Msix ^
  -p:AppxPackageDir="../output/" ^
  -p:PackageCertificateStoreLocation="CurrentUser" ^
  -p:PackageCertificateStoreName="My" ^
  -p:PackageCertificateThumbprint=52E6E26AD745DE7F7EB2CDC031509D57F78CEBF8 ^
  -p:PackageCertificatePassword=kor ^
  -v diag

  REM -p:PackageCertificateKeyFile="KoOrderRegister\Technical\kor.pfx" ^
  REM -p:PackageCertificatePassword=kor
  REM -p:PackageCertificateThumbprint=52E6E26AD745DE7F7EB2CDC031509D57F78CEBF8
  REM -p:PackageCertificateStoreLocation="LocalMachine" ^
  REM -p:PackageCertificateStoreName="My" ^
  REM -v diag ^

  REM -p:PackageCertificateKeyFile="KoOrderRegister/Technical/kor.pfx" ^
  REM -p:PackageCertificatePassword="kor" ^
echo msix file %OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test
echo Copying MSIX package to general output directory...



if "%BUILD_VERSION%"=="DEV_VERSION" (
set "msix_folder=%OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Debug_Test"
    REM Find the first .msix file
for /f "delims=" %%f in ('dir /b "%OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Debug_Test\*.msix"') do (
    set "msixFile=%%f"
    goto :FoundMsix
)
) else (

set "msix_folder=%OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test"

    REM Find the first .msix file
for /f "delims=" %%f in ('dir /b "%OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test\*.msix"') do (
    set "msixFile=%%f"
    goto :FoundMsix
)
)

echo Msix folder: %msix_folder%
echo No msix file found in %msix_folder%
exit /b 1

:FoundMsix
echo Msix folder: %msix_folder%

set "newFileName=KoOrderRegister_%WINDOWS_NEW_VERSION%_X64_%BUILD_VERSION%.msix"
echo New File name: %newFileName%

if defined msixFile (
    move "%msix_folder%\%msixFile%" "%OUTPUT_DIR%\%newFileName%"
    echo File renamed: %OUTPUT_DIR%\%newFileName%
) else (
    echo No msix file in %OUTPUT_DIR%
	echo New File name: %newFileName%
)

echo Copy completed.
echo MSIX build process completed.

endlocal
