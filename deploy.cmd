bower install
if EXIST "Web.config" (
 @ECHO OFF
  PowerShell.exe -Command "& '.\deploy.ps1'"
 PAUSE
)