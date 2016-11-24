$ServerInstance = "(localdb)\mssqllocaldb"
$DbName = "Basniowa"
$ScriptPath = split-path $SCRIPT:MyInvocation.MyCommand.Path -parent
echo $ScriptPath
$DbPath = Join-Path $ScriptPath "LocalDb"
echo "Ensuring DB directory exists ($DbPath)"
New-Item -ItemType Directory -Force -Path $DbPath > $null

Push-Location
Import-Module "sqlps" -DisableNameChecking
Pop-Location

$Variables = "DbName = $DbName", "DbPath = $DbPath"
echo "Recreating DB"
Invoke-Sqlcmd -InputFile "$ScriptPath\scripts\_recreate.sql" -ServerInstance $ServerInstance -Variable $Variables -AbortOnError

$Scripts = Get-ChildItem -Path "$ScriptPath\scripts" -Filter "script_*.sql" | Sort-Object
foreach ($Script in $Scripts) {
    echo "Executing script: $Script"
    Invoke-Sqlcmd -InputFile "$ScriptPath\scripts\$Script" -ServerInstance $ServerInstance -Database $DbName -AbortOnError -ErrorAction Stop
}