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
    if ($m.Name -eq 'a5') {
        $params = $m.GetParameters() | ForEach-Object { "$($_.Name): $($_.ParameterType.Name)" }
        $paramStr = $params -join ', '
        $body = $m.GetMethodBody()
        $ilSize = if ($body) { $body.GetILAsByteArray().Length } else { 0 }
        Write-Host "a5 OVERLOAD: a5($paramStr) -> Return: $($m.ReturnType.Name), IL Size: $ilSize bytes"
        
        if ($body -and $ilSize -gt 1000) {
            # Check string tokens in this method
            $bytes = $body.GetILAsByteArray()
            for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                if ($bytes[$i] -eq 0x72) { # ldstr
                    $token = [BitConverter]::ToInt32($bytes, $i + 1)
                    try {
                        $str = $module.ResolveString($token)
                        if ($str -like '*UPDATE *' -or $str -like '*INSERT INTO*' -or $str -like '*FCT_*') {
                            Write-Host "   -> SQL IN METHOD: $str"
                        }
                    } catch {}
                }
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
