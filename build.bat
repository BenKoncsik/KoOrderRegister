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




echo Building MSIX package for Windows x64...
REM dotnet publish ".\KoOrderRegister\KoOrderRegister.csproj" -r win-x64 -c Release -f net8.0-windows10.0.19041.0 --output "output/build/" -p:PackageType=Msix -p:PackageCertificateKeyFile="Technical\kor.pfx" -p:PackageCertificatePassword="kor" -p:PackageCertificateThumbprint="52E6E26AD745DE7F7EB2CDC031509D57F78CEBF8" -v diag -p:AppxPackageDir="../output/"

dotnet publish ".\KoOrderRegister\KoOrderRegister.csproj" ^
  -r win-x64 -c Release -f net8.0-windows10.0.19041.0 ^
  --output "output/build/" ^
  -p:PackageType=Msix ^
  -p:PackageCertificateKeyFile="Technical/kor.pfx" ^
  -p:PackageCertificatePassword="kor" ^
  -v diag ^
  -p:AppxPackageDir="../output/"


echo msix file $OUTPUT_DIR\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test
echo Copying MSIX package to general output directory...
$msixFile = Get-ChildItem -Path $OUTPUT_DIR\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test -Filter "*.msix" | Select-Object -First 1



if ($msixFile) {
    # Új fájlnév létrehozása
    $newFileName = "KoOrderRegister_$NEW_VERSION_X64.msix"
    $newFilePath = Join-Path $OUTPUT_DIR $newFileName

    # Fájl átnevezése
    Rename-Item -Path $msixFile.FullName -NewName $newFileName
    Write-Host "File renamed: $newFilePath"
} else {
    Write-Host "No msix file $OUTPUT_DIR"
}
xcopy "%OUTPUT_DIR%\KoOrderRegister_%WINDOWS_NEW_VERSION%_Test\*.msix" "%OUTPUT_DIR%" /Y /I
echo Copy completed.
echo MSIX build process completed.

endlocal
