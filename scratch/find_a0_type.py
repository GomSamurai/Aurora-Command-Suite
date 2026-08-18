import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$types = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -or $_.Name -like '*a0*' }
foreach ($t in $types) {
    Write-Host "TYPE MATCH: "$t.FullName
    $methods = $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')
    foreach ($m in $methods) {
        Write-Host "   Method: $($m.Name) (Params: $($m.GetParameters().Length))"
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
