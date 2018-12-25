@echo off

title %~df0

if "%~d0" neq "C:" (
  echo.
  echo ############################################
  echo   Please run this script on a local drive.
  echo ############################################
  echo.
  pause
  exit /b
)

cd /d %~dp0

set SQLCMDSERVER=TOKRV3006SQL1.JAPAN.NOM\TK_MS_FTABI_PRD1,2500
set SQLCMDDBNAME=EQ_VERSION_MANAGER
set SQLCMDUSER=eq_sp_admin
set SQLCMDPASSWORD=IWm@L8ga

set /p answer=Do you wish to modify the stored procedure with %SQLCMDDBNAME% on %SQLCMDSERVER% ? (y/N):
if /i not %answer% == y exit

sqlcmd -f 932 -i %~n0.sql > result.txt

echo Done!

pause
