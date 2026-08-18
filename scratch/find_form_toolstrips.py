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
        $toolstrips = $fields | Where-Object { [System.Windows.Forms.ToolStrip]::IsAssignableFrom($_.FieldType) }
        $buttons = $fields | Where-Object { [System.Windows.Forms.Button]::IsAssignableFrom($_.FieldType) }
        if ($toolstrips.Count -gt 0 -or $buttons.Count -gt 15) {
            Write-Host "FORM: $($t.FullName) -> ToolStrips: $($toolstrips.Count), Buttons: $($buttons.Count)"
            foreach ($ts in $toolstrips) {
                Write-Host "   ToolStrip Field: $($ts.Name)"
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
