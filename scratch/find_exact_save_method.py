import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

Write-Host "--- Searching all classes in Aurora.exe for FCT_Population or FCT_Game save SQL ---"

foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                $bytes = $body.GetILAsByteArray()
                for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                    if ($bytes[$i] -eq 0x72) { # ldstr
                        $token = [BitConverter]::ToInt32($bytes, $i + 1)
                        try {
                            $str = $t.Module.ResolveString($token)
                            if ($str -like '*FCT_Population*' -or $str -like '*FCT_PopulationInstallations*' -or $str -like '*UPDATE FCT_Game*') {
                                $params = $m.GetParameters() | ForEach-Object { "$($_.Name): $($_.ParameterType.Name)" }
                                Write-Host "EXACT SAVE METHOD: Class '$($t.FullName)' -> Method '$($m.Name)' ($($params -join ', ')) | IL Size: $($bytes.Length) bytes | SQL: $str"
                            }
                        } catch {}
                    }
                }
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total lines: {len(lines)}")
for l in lines[:100]:
    print(l)
