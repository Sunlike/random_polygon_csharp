SET FrameworkDir=C:\WINDOWS\Microsoft.NET\Framework\
SET FrameworkVersion=v3.5
SET PATH=%FrameworkDir%%FrameworkVersion%;%PATH%
SET CurWorkPath=%~dp0
SET NSISDir=%CurWorkPath%\NSIS
SET PATH=%NSISDir%;%PATH%


:ɾ���ϵ��ļ�
:call Clear.bat

:����
echo ----------------------------------------------------------------
MSBuild /p:Configuration="Release" /t:Rebuild Random_Polygon.sln

 
echo ----------------------------------------------------------------
echo %date% %time% ��ʼ����ͻ���
makensis.exe /V1 ClientSetup.nsi