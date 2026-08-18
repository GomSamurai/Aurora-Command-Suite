import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
        if ($m.Name -like '*Save*' -or $m.Name -like '*Load*' -or $m.Name -like '*DB*' -or $m.Name -like '*Refresh*' -or $m.Name -like '*Populate*') {
            Write-Host "$($t.Name) :: $($m.Name)"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
output_lines = res.stdout.splitlines()
print(f"Total methods found: {len(output_lines)}")
for line in output_lines[:60]:
    print(line)
