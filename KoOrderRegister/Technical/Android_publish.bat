@echo off
setlocal

REM A script aktuális mappájából indítva
set "CS_PROJECT=..\KoOrderRegister.csproj"

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
powershell -Command "(gc '%CS_PROJECT%') -replace '<ApplicationDisplayVersion>%CURRENT_VERSION%</ApplicationDisplayVersion>', '<ApplicationDisplayVersion>%NEW_VERSION%</ApplicationDisplayVersion>' | Out-File -encoding UTF8 '%CS_PROJECT%'"

echo Publishing the application...
dotnet publish "..\KoOrderRegister.sln" -c Release -f net8.0-android

REM Cél APK útvonal
set "ORIGINAL_APK=..\bin\Release\net8.0-android\hu.kncsk.koorderregister-Signed.apk"

REM Új APK neve a frissített verzióval
set "NEW_APK_NAME=..\bin\Release\net8.0-android\KoOrderRegister_%NEW_VERSION%_android.apk"

echo Renaming the APK file...
move "%ORIGINAL_APK%" "%NEW_APK_NAME%"

echo Build and rename process completed. New APK: %NEW_APK_NAME%

endlocal


