import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$form1 = $asm.GetType('Form1')
if ($form1 -eq $null) {
    $form1 = $asm.GetTypes() | Where-Object { $_.Name -eq 'Form1' -or $_.Name -eq 'MainForm' }
}

Write-Host "Form1 Type: "$form1.FullName

foreach ($f in $form1.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance')) {
    if ([System.Windows.Forms.ToolStrip]::IsAssignableFrom($f.FieldType) -or [System.Windows.Forms.Control]::IsAssignableFrom($f.FieldType)) {
        Write-Host "CONTROL FIELD: "$f.Name " (" $f.FieldType.Name ")"
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
