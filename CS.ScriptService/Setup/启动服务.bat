
@echo off

echo 注意‘服务名称’与服务 '*.exe'的差别
set SvcName=CSScriptServiceV4

echo 即将启动服务%SvcName%
net start %SvcName%

echo.
pause