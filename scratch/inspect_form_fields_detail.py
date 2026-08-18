import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

foreach ($tName in @('Form1', 'hf', 'f5', 'bd', 'kn')) {
    $t = $asm.GetType($tName)
    if ($t -ne $null) {
        Write-Host "=== CLASS: $($t.FullName) ==="
        $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
        foreach ($f in $fields) {
            Write-Host "   Field: $($f.Name) -> Type: $($f.FieldType.FullName)"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
