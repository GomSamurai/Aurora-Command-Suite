import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

Write-Host "--- Searching all classes and methods in Aurora.exe for DB calls ---"

foreach ($t in $asm.GetTypes()) {
    $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
    foreach ($f in $fields) {
        if ($f.FieldType.Name -like '*Connection*' -or $f.FieldType.Name -like '*SQLite*') {
            Write-Host "TYPE WITH SQLITE FIELD: $($t.FullName) -> Field: $($f.Name) ($($f.FieldType.FullName))"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
