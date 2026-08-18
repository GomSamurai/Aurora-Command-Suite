import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
$baseForm = [System.Windows.Forms.Form]

foreach ($t in $asm.GetTypes()) {
    if ($baseForm.IsAssignableFrom($t)) {
        Write-Host "FORM CLASS: "$t.FullName
        foreach ($f in $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance')) {
            if ([System.Windows.Forms.Button]::IsAssignableFrom($f.FieldType)) {
                Write-Host "   Button Field: "$f.Name
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
output_lines = res.stdout.splitlines()
print(f"Total lines: {len(output_lines)}")
for line in output_lines[:50]:
    print(line)
