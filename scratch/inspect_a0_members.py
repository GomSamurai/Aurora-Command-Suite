import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }
Write-Host "FOUND TYPE: "$a0.FullName

$members = $a0.GetMembers([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
Write-Host "Total Members: "$members.Count
foreach ($m in $members) {
    if ($m.MemberType -eq 'Method') {
        Write-Host "METHOD: "$m.Name
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
