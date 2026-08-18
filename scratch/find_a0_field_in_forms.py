import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0Type = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }
Write-Host "a0 Type FullName: "$a0Type.FullName " BaseType: "$a0Type.BaseType.FullName

foreach ($t in $asm.GetTypes()) {
    if ([System.Windows.Forms.Form]::IsAssignableFrom($t)) {
        $fields = $t.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')
        foreach ($f in $fields) {
            if ($a0Type.IsAssignableFrom($f.FieldType) -or $f.FieldType -eq $a0Type) {
                Write-Host "MATCH FIELD: Form $($t.Name) -> Field '$($f.Name)' (FieldType: $($f.FieldType.FullName))"
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
