Push-Location
..\Database\recreate_db.ps1
cd ..\Website
$WebHost = Start-Process powershell {dotnet run;} -passthru
Pop-Location

Start-Sleep -Seconds 5

newman run Scenario_Empty.json -e Env_Local.json

$WebHost.CloseMainWindow();
pause