import subprocess

ps_script = """
$bytes = [System.IO.File]::ReadAllBytes('C:\\VSCODE\\Aurora271Full\\Patches\\Lib\\Lib.dll')
$asm = [System.Reflection.Assembly]::Load($bytes)
Write-Host "Assembly Name: "$asm.FullName
foreach ($t in $asm.GetExportedTypes()) {
    Write-Host $t.FullName
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
print("STDERR:", res.stderr)
