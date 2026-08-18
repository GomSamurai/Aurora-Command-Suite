import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
foreach ($r in $asm.GetReferencedAssemblies()) {
    Write-Host "REF: "$r.FullName
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
