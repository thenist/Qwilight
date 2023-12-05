@ECHO OFF

SET /P DATE=v

SET VS2022=%PROGRAMFILES%\Microsoft Visual Studio\2022\Community
SET MSBUILD=%VS2022%\Msbuild\Current\Bin\MSBuild.exe
SET BANDIZIP=%PROGRAMFILES%\Bandizip\bz.exe
SET WINX64=bin\x64\Release\net8.0-windows10.0.22621.0\win-x64
SET PUBLISH=Qwilight\%WINX64%\publish

DEL Qwilight.zip
 
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

RMDIR /S /Q %PUBLISH%
dotnet publish Qwilight\Qwilight.csproj -c Release -p:Platform=x64
"%BANDIZIP%" c -storeroot:no Qwilight.zip %PUBLISH%
	
powershell $(CertUtil -hashfile %PUBLISH%\Qwilight.dll SHA512)[1] > Qwilight.dll.sha512sum
SET /P BUILD= < Qwilight.dll.sha512sum
DEL Qwilight.dll.sha512sum

CHOICE /M PATCH
IF %ERRORLEVEL% == 1 (
	Robocopy . \\taehui\taehui Qwilight.zip
	wsl ssh taehui@taehui sudo chown root:root Qwilight.zip
	wsl ssh taehui@taehui sudo mv Qwilight.zip /var/www/html/qwilight/Qwilight.zip
	curl -X PATCH taehui:8300/date -d "%DATE% %BUILD%"
)

CHOICE /M VALVE
SET VALVE=%ERRORLEVEL%
IF %VALVE% == 1 (
	SET /P ID=ID: 
	SET /P PW=PW: 
)
IF %VALVE% == 1 (
	Qwilight\sdk\tools\ContentBuilder\builder\steamcmd +login %ID% %PW% +run_app_build ..\scripts\simple_app_build.vdf +quit
)
IF %VALVE% == 1 (
	IF %ERRORLEVEL% == 0  (
		start https://partner.steamgames.com/apps/builds/1910130
	)
)

CHOICE /M VCS
IF %ERRORLEVEL% == 1 (
	git add *
	git commit -m "v%DATE% (%BUILD%)"
	git checkout master
	git merge develop
	git push
	git checkout develop
	git push
)