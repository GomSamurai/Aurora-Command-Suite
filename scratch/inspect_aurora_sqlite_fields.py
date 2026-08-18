import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
foreach ($t in $asm.GetTypes()) {
    $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
    foreach ($f in $fields) {
        if ($f.FieldType.Name -like '*SQLite*' -or $f.Name -like '*DB*' -or $f.Name -like '*Conn*') {
            Write-Host "$($t.Name) :: $($f.Name) ($($f.FieldType.Name))"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
