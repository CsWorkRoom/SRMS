@echo off

echo ע�⡮�������ơ���Ӧ�ó������� '*.exe'�Ĳ��
set SvcName=CSScriptServiceV4

echo ����ֹͣ����%SvcName%
net stop %SvcName%

set ExeName=CS.ScriptService.exe

echo ����ж��Ӧ�ó���%ExeName% �ķ���
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe %~dp0\%ExeName% -u

echo.
pause 