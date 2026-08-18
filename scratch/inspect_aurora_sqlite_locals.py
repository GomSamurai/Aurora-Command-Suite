import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                foreach ($local in $body.LocalVariables) {
                    if ($local.LocalType.Name -like '*SQLite*') {
                        Write-Host "$($t.Name) :: $($m.Name) -> $($local.LocalType.Name)"
                    }
                }
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
output = res.stdout.splitlines()
print(f"Total method matches with SQLite locals: {len(output)}")
for line in output[:40]:
    print(line)
