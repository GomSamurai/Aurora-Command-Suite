import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    try {
        $body = $m.GetMethodBody()
        if ($body -ne $null) {
            $bytes = $body.GetILAsByteArray()
            if ($bytes.Length -gt 500) { # Large save methods are hundreds of bytes
                Write-Host "LARGE ENGINE METHOD: $($m.Name) (IL Size: $($bytes.Length) bytes, Params: $($m.GetParameters().Length))"
            }
        }
    } catch {}
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total matching methods: {len(lines)}")
for l in lines[:100]:
    print(l)
