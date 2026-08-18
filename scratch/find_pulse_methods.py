import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0Type = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

# Look for methods on a0 taking double/decimal/int (increment length in seconds)
foreach ($m in $a0Type.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    $params = $m.GetParameters()
    if ($params.Length -eq 1) {
        $pType = $params[0].ParameterType.Name
        if ($pType -eq 'Decimal' -or $pType -eq 'Double' -or $pType -eq 'Int64' -or $pType -eq 'Int32') {
            Write-Host "PULSE METHOD CANDIDATE: a0::$($m.Name) (Param: $pType)"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
