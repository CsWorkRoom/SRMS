
@echo off

echo ע�⡮�������ơ���Ӧ�ó������� '*.exe'�Ĳ��
set ExeName=CS.ScriptService.exe

echo ������װӦ�ó���%ExeName% Ϊ����
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe %~dp0\%ExeName%

echo.
pause
