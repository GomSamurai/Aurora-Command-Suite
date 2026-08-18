import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

$saveTokens = @()
foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    if ($m.Name -eq 'ji' -or $m.Name -eq 'jk') {
        $saveTokens += $m.MetadataToken
        Write-Host "Save Method: $($m.Name) (Token: $($m.MetadataToken))"
    }
}

foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    try {
        $body = $m.GetMethodBody()
        if ($body -ne $null) {
            $bytes = $body.GetILAsByteArray()
            for ($i = 0; $i -lt $bytes.Length - 4; $i++) {
                if ($bytes[$i] -eq 0x28 -or $bytes[$i] -eq 0x6f) {
                    $token = [BitConverter]::ToInt32($bytes, $i + 1)
                    if ($saveTokens -contains $token) {
                        Write-Host ">>> MASTER SAVE CALLER FOUND: Method '$($m.Name)' (Params: $($m.GetParameters().Length), IL Size: $($bytes.Length) bytes)"
                        break
                    }
                }
            }
        }
    } catch {}
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
