import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }
Write-Host "FOUND TYPE: "$a0.FullName

$methods = $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')
Write-Host "Total Methods: "$methods.Count
foreach ($m in $methods) {
    Write-Host "METHOD: "$m.Name " (Params: "$m.GetParameters().Length ")"
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
