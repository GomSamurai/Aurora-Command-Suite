import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
foreach ($t in $asm.GetTypes()) {
    if ([System.Windows.Forms.Form]::IsAssignableFrom($t)) {
        Write-Host $t.FullName
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
