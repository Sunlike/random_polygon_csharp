SET FrameworkDir=C:\WINDOWS\Microsoft.NET\Framework\
SET FrameworkVersion=v3.5
SET PATH=%FrameworkDir%%FrameworkVersion%;%PATH%
SET CurWorkPath=%~dp0
SET NSISDir=%CurWorkPath%\NSIS
SET PATH=%NSISDir%;%PATH%


:删除老的文件
:call Clear.bat

:编译
echo ----------------------------------------------------------------
MSBuild /p:Configuration="Release" /t:Rebuild Random_Polygon.sln

 
echo ----------------------------------------------------------------
echo %date% %time% 开始打包客户端
makensis.exe /V1 ClientSetup.nsi