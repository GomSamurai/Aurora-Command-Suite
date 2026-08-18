import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

$targetTokens = @()
foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    if ($m.Name -eq 'o6' -or $m.Name -eq 'iy') {
        if ($m.GetParameters().Length -eq 0) {
            $targetTokens += $m.MetadataToken
            Write-Host "Target Method: $($m.Name) (Token: $($m.MetadataToken))"
        }
    }
}

foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                $bytes = $body.GetILAsByteArray()
                for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                    if ($bytes[$i] -eq 0x28 -or $bytes[$i] -eq 0x6f) {
                        $token = [BitConverter]::ToInt32($bytes, $i + 1)
                        if ($targetTokens -contains $token) {
                            Write-Host "CALLER FOUND: Class '$($t.FullName)' -> Method '$($m.Name)' (Params: $($m.GetParameters().Length), IL Size: $($bytes.Length) bytes)"
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
