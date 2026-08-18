import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

Write-Host "--- Searching classes in Aurora.exe with SQLiteConnection ---"

foreach ($t in $asm.GetTypes()) {
    $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
    foreach ($f in $fields) {
        if ($f.FieldType.Name -eq 'SQLiteConnection') {
            Write-Host "CLASS WITH SQLITE CONN: $($t.FullName) -> Field: $($f.Name)"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
