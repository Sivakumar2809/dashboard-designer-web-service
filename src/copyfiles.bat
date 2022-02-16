@echo off
setlocal EnableDelayedExpansion
cd ..\packages
set "folderName="
cd syncfusion.ejdashboarddesigner 
echo "%foldername%"
call:findLatestPackage folderName

pushd %folderName%
echo d | xcopy "content\scripts\*.*" "..\..\..\src\scripts"/f /s /y /r
echo d | xcopy "content\themes\*.*" "..\..\..\src\themes"/f /s /y /r
echo d | xcopy "content\localization\*.*" "..\..\..\src\localization"/f /s /y /r
echo d | xcopy "content\locale\*.*" "..\..\..\src\locale"/f /s /y /r

cd ../../syncfusion.ej1.widgets
echo "%CD%
call:findLatestPackage folderName
pushd %folderName%
echo d | xcopy "content\scripts\*.*" "..\..\..\src\scripts"/f /s /y /r
echo d | xcopy "content\themes\*.*" "..\..\..\src\themes"/f /s /y /r

cd ../../syncfusion.ej2.widgets
call:findLatestPackage folderName
pushd %folderName%
echo d | xcopy "content\scripts\*.*" "..\..\..\src\scripts"/f /s /y /r
echo d | xcopy "content\themes\*.*" "..\..\..\src\themes"/f /s /y /r
goto:eof




:findLatestPackage

set "dname="
FOR /D %%F IN (*) DO (
set "dname=%%F"
echo %%F
)
set %~1=%dname%
