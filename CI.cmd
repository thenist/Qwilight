@ECHO OFF

SET VS2022=%PROGRAMFILES%\Microsoft Visual Studio\2022\Community
SET MSBUILD=%VS2022%\Msbuild\Current\Bin\MSBuild.exe

"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=x64 Flint\Flint.vcxproj
Robocopy Flint\x64\Release Qwilight\Assets\AMD64 Flint.exe
"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=ARM64 Flint\Flint.vcxproj
Robocopy Flint\ARM64\Release Qwilight\Assets\ARM64 Flint.exe

"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=x64 Igniter\Igniter.csproj
Robocopy Igniter\bin\x64\Release Qwilight\Assets\AMD64 Igniter.exe
"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=ARM64 Igniter\Igniter.csproj
Robocopy Igniter\bin\ARM64\Release Qwilight\Assets\ARM64 Igniter.exe

"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=x64 NVIDIA\NVIDIA.vcxproj
Robocopy NVIDIA\x64\Release Qwilight\Assets\AMD64 NVIDIA.dll

"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=x64 Xwindow\Xwindow.vcxproj
Robocopy Xwindow\x64\Release Qwilight\Assets\AMD64 Xwindow.exe
"%MSBUILD%" -t:Build -p:Configuration=Release,Platform=ARM64 Xwindow\Xwindow.vcxproj
Robocopy Xwindow\ARM64\Release Qwilight\Assets\ARM64 Xwindow.exe

IF %PROCESSOR_ARCHITECTURE% == AMD64 (
	dotnet build Language\Language.csproj -c Release -p:Platform=x64
	Language\bin\x64\Release\net8.0\Language.exe
)
IF %PROCESSOR_ARCHITECTURE% == ARM64 (
	dotnet build Language\Language.csproj -c Release -p:Platform=ARM64
	Language\bin\ARM64\Release\net8.0\Language.exe
)
