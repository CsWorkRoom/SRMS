@echo off

echo 注意‘服务名称’与应用程序名称 '*.exe'的差别
set SvcName=CSScriptServiceV4

echo 即将停止服务%SvcName%
net stop %SvcName%

set ExeName=CS.ScriptService.exe

echo 即将卸载应用程序%ExeName% 的服务
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe %~dp0\%ExeName% -u

echo.
pause 