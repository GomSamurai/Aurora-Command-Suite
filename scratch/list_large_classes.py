import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

foreach ($t in $asm.GetTypes()) {
    $methods = $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')
    $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
    if ($methods.Count -gt 50 -or $fields.Count -gt 50) {
        Write-Host "LARGE CLASS: $($t.FullName) -> Methods: $($methods.Count), Fields: $($fields.Count)"
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
