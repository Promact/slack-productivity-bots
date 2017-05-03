cd Slack.Automation\Promact.Erp.Web
npm run aot & npm run rollup
if EXIST "Web.config" (
 @ECHO OFF
  PowerShell.exe -Command "& '.\deploy.ps1'"
 PAUSE
)