import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
$baseForm = [System.Windows.Forms.Form]

foreach ($t in $asm.GetTypes()) {
    if ($baseForm.IsAssignableFrom($t)) {
        $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance')
        Write-Host "FORM: "$t.FullName " (Fields: "$fields.Count ")"
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total matching Forms: {len(lines)}")
for l in lines[:100]:
    print(l)
