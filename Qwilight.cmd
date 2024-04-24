@ECHO OFF

SET /P DATE=v

SET VS2022=%PROGRAMFILES%\Microsoft Visual Studio\2022\Community
SET MSBUILD=%VS2022%\Msbuild\Current\Bin\MSBuild.exe
SET BANDIZIP=%PROGRAMFILES%\Bandizip\bz.exe
SET DOWNLOADS=%USERPROFILE%\Downloads
SET WINX64=bin\x64\Release\net8.0-windows10.0.22621.0\win-x64
SET WINARM64=bin\ARM64\Release\net8.0-windows10.0.22621.0\win-arm64
SET WINX64PUBLISH=Qwilight\%WINX64%\publish
SET WINARM64PUBLISH=Qwilight\%WINARM64%\publish
 
CALL CI

CHOICE /M TEST
SET TEST=%ERRORLEVEL%
IF %TEST% == 1 (
	RMDIR /S /Q Test\%WINX64%
	Robocopy Test\qpdgo\Bundle Test\%WINX64%\qpdgo\Bundle /MIR
	MKDIR Test\%WINX64%\qpdgo\UI
	"%BANDIZIP%" x -target:auto Test\qpdgo\UI\*.zip Test\%WINX64%\qpdgo\UI
)
IF %TEST% == 1 (
	dotnet test Test\Test.csproj -c Release -p:Platform=x64
)
IF %TEST% == 1 (
	IF NOT %ERRORLEVEL% == 0  (
		PAUSE
	)
)

RMDIR /S /Q %WINX64PUBLISH%
dotnet publish Qwilight\Qwilight.csproj -c Release -p:Platform=x64
"%BANDIZIP%" c -storeroot:no %DOWNLOADS%\Qwilight.X64.zip %WINX64PUBLISH%
	
powershell $(CertUtil -hashfile %WINX64PUBLISH%\Qwilight.dll SHA512)[1] > Qwilight.dll.sha512sum
SET /P HASH_X64= < Qwilight.dll.sha512sum
DEL Qwilight.dll.sha512sum

RMDIR /S /Q %WINARM64PUBLISH%
dotnet publish Qwilight\Qwilight.csproj -c Release -p:Platform=ARM64
"%BANDIZIP%" c -storeroot:no %DOWNLOADS%\Qwilight.ARM64.zip %WINARM64PUBLISH%

powershell $(CertUtil -hashfile %WINARM64PUBLISH%\Qwilight.dll SHA512)[1] > Qwilight.dll.sha512sum
SET /P HASH_ARM64= < Qwilight.dll.sha512sum
DEL Qwilight.dll.sha512sum

CHOICE /M PATCH
IF %ERRORLEVEL% == 1 (
	curl -X PATCH taehui:8300/date/X64 --data-binary "@%DOWNLOADS%\Qwilight.X64.zip"
	curl -X PATCH taehui:8300/date/ARM64 --data-binary "@%DOWNLOADS%\Qwilight.ARM64.zip"
	curl -X PATCH taehui:8300/date -d "%DATE% %HASH_X64% %HASH_ARM64%"
)

CHOICE /M VALVE
SET VALVE=%ERRORLEVEL%
IF %VALVE% == 1 (
	SET /P ID=ID: 
	SET /P PW=PW: 
)
IF %VALVE% == 1 (
	sdk\tools\ContentBuilder\builder\steamcmd +login %ID% %PW% +run_app_build ..\scripts\simple_app_build.vdf +quit
)
IF %VALVE% == 1 (
	IF %ERRORLEVEL% == 0  (
		start https://partner.steamgames.com/apps/builds/1910130
	)
)

CHOICE /M VCS
IF %ERRORLEVEL% == 1 (
	git add *
	git commit -m "v%DATE%"
	git push
	git checkout master
	git merge develop
	git push
	git checkout develop
)