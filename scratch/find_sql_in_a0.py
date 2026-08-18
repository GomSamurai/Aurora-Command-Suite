import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }
$module = $a0.Module

foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    try {
        $body = $m.GetMethodBody()
        if ($body -ne $null) {
            $bytes = $body.GetILAsByteArray()
            for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                if ($bytes[$i] -eq 0x72) { # ldstr opcode
                    $token = [BitConverter]::ToInt32($bytes, $i + 1)
                    try {
                        $str = $module.ResolveString($token)
                        if ($str -like '*UPDATE *' -or $str -like '*INSERT INTO*' -or $str -like '*DELETE FROM*') {
                            Write-Host "METHOD WITH SQL: $($m.Name) (Params: $($m.GetParameters().Length)) -> String: $str"
                        }
                    } catch {}
                }
            }
        }
    } catch {}
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
