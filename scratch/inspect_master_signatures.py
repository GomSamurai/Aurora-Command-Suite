import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    if ($m.Name -eq 'a5' -or $m.Name -eq 'a6') {
        $body = $m.GetMethodBody()
        if ($body -ne $null -and $body.GetILAsByteArray().Length -gt 15000) {
            $params = $m.GetParameters() | ForEach-Object { "$($_.Name): $($_.ParameterType.FullName)" }
            Write-Host "MASTER METHOD: $($m.Name) -> Params: $($params -join ', ') (IsStatic: $($m.IsStatic))"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
