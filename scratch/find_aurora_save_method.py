import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
$baseForm = [System.Windows.Forms.Form]

foreach ($t in $asm.GetTypes()) {
    if ($baseForm.IsAssignableFrom($t)) {
        $saveMethods = $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly') | Where-Object { $_.Name -like '*Save*' -or $_.Name -like '*DB*' -or $_.Name -like '*Write*' -or $_.Name -like '*Click*' }
        if ($saveMethods.Count -gt 0) {
            Write-Host "FORM: "$t.FullName
            foreach ($m in $saveMethods) {
                Write-Host "   -> $($m.Name) (Params: $($m.GetParameters().Length))"
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
