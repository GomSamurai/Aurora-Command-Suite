import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

$zeroMethods = @('pe', 'o7', 'oe', 'og', 'ok', 'ol', 'on')

foreach ($mName in $zeroMethods) {
    $methods = $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static') | Where-Object { $_.Name -eq $mName -and $_.GetParameters().Length -eq 0 }
    foreach ($m in $methods) {
        Write-Host "=== METHOD $($m.Name) ==="
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                Write-Host "IL Size: "$body.GetILAsByteArray().Length " bytes"
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
