@echo off

echo ע�⡮�������ơ������ '*.exe'�Ĳ��

set SvcName=CSScriptServiceV4
echo Service state: %SvcName%
sc query %SvcName%

echo.
pause