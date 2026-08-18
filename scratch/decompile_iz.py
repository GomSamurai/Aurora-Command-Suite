import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

$m = $a0.GetMethod('iz', [System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static', $null, @(), $null)
Write-Host "Method iz: IL Size $($m.GetMethodBody().GetILAsByteArray().Length) bytes"

# Check callers of iz
$token = $m.MetadataToken
foreach ($t in $asm.GetTypes()) {
    foreach ($meth in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
        try {
            $body = $meth.GetMethodBody()
            if ($body -ne $null) {
                $bytes = $body.GetILAsByteArray()
                for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                    if ($bytes[$i] -eq 0x28 -or $bytes[$i] -eq 0x6f) {
                        $tTok = [BitConverter]::ToInt32($bytes, $i + 1)
                        if ($tTok -eq $token) {
                            Write-Host "CALLER OF iz(): Class '$($t.FullName)' -> Method '$($meth.Name)' (IL Size: $($bytes.Length) bytes)"
                        }
                    }
                }
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
