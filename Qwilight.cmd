@ECHO OFF

SET /P DATE=v

SET VS2022=%PROGRAMFILES%\Microsoft Visual Studio\2022\Community
SET MSBUILD=%VS2022%\Msbuild\Current\Bin\MSBuild.exe
SET BANDIZIP=%PROGRAMFILES%\Bandizip\bz.exe
SET DOWNLOADS=%USERPROFILE%\Downloads
SET WINAMD64=bin\x64\Release\net8.0-windows10.0.22621.0\win-x64
SET WINARM64=bin\ARM64\Release\net8.0-windows10.0.22621.0\win-arm64
SET WINAMD64PUBLISH=Qwilight\%WINAMD64%\publish
SET WINARM64PUBLISH=Qwilight\%WINARM64%\publish
 
CALL CI

CHOICE /M TEST
SET TEST=%ERRORLEVEL%
IF %TEST% == 1 (
	IF %PROCESSOR_ARCHITECTURE% == AMD64 (
		RMDIR /S /Q Test\%WINAMD64%
		Robocopy Test\qpdgo\Bundle Test\%WINAMD64%\qpdgo\Bundle /MIR
		MKDIR Test\%WINAMD64%\qpdgo\UI
		"%BANDIZIP%" x -target:auto Test\qpdgo\UI\*.zip Test\%WINAMD64%\qpdgo\UI
		dotnet test Test\Test.csproj -c Release -p:Platform=x64
	)
	IF %PROCESSOR_ARCHITECTURE% == ARM64 (
		RMDIR /S /Q Test\%WINARM64%
		Robocopy Test\qpdgo\Bundle Test\%WINARM64WINARM64%\qpdgo\Bundle /MIR
		MKDIR Test\%WINARM64%\qpdgo\UI
		"%BANDIZIP%" x -target:auto Test\qpdgo\UI\*.zip Test\%WINARM64%\qpdgo\UI
		dotnet test Test\Test.csproj -c Release -p:Platform=ARM64
	)
)
IF %TEST% == 1 (
	IF NOT %ERRORLEVEL% == 0  (
		PAUSE
	)
)

RMDIR /S /Q %WINAMD64PUBLISH%
dotnet publish Qwilight\Qwilight.csproj -c Release -p:Platform=x64
"%BANDIZIP%" c -storeroot:no %DOWNLOADS%\Qwilight.AMD64.zip %WINAMD64PUBLISH%
	
powershell $(CertUtil -hashfile %WINAMD64PUBLISH%\Qwilight.dll SHA512)[1] > Qwilight.dll.sha512sum
SET /P HASH_AMD64= < Qwilight.dll.sha512sum
DEL Qwilight.dll.sha512sum

RMDIR /S /Q %WINARM64PUBLISH%
dotnet publish Qwilight\Qwilight.csproj -c Release -p:Platform=ARM64
"%BANDIZIP%" c -storeroot:no %DOWNLOADS%\Qwilight.ARM64.zip %WINARM64PUBLISH%

powershell $(CertUtil -hashfile %WINARM64PUBLISH%\Qwilight.dll SHA512)[1] > Qwilight.dll.sha512sum
SET /P HASH_ARM64= < Qwilight.dll.sha512sum
DEL Qwilight.dll.sha512sum

CHOICE /M PATCH
IF %ERRORLEVEL% == 1 (
	curl -X PATCH taehui:8300/date/AMD64 --data-binary "@%DOWNLOADS%\Qwilight.AMD64.zip"
	curl -X PATCH taehui:8300/date/ARM64 --data-binary "@%DOWNLOADS%\Qwilight.ARM64.zip"
	curl -X PATCH taehui:8300/date -d "%DATE% %HASH_AMD64% %HASH_ARM64%"
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