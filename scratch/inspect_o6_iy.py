import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

foreach ($name in @('o6', 'iy')) {
    $m = $a0.GetMethod($name, [System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static', $null, @(), $null)
    if ($m) {
        Write-Host "\n=== METHOD: $name () ==="
        $body = $m.GetMethodBody()
        $bytes = $body.GetILAsByteArray()
        Write-Host "IL Length: $($bytes.Length) bytes"
        
        for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
            if ($bytes[$i] -eq 0x72) {
                $token = [BitConverter]::ToInt32($bytes, $i + 1)
                try {
                    $str = $a0.Module.ResolveString($token)
                    Write-Host "   -> String Token: '$str'"
                } catch {}
            }
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
