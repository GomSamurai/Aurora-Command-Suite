import subprocess

ps_script = """
Add-Type -AssemblyName System.Windows.Forms
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

$a0 = $asm.GetTypes() | Where-Object { $_.Name -eq 'a0' -and $_.DeclaringType -eq $null }

# Search for single zero-parameter method or single-parameter method on a0 that calls multiple kz, ko, etc.
foreach ($m in $a0.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static')) {
    if ($m.GetParameters().Length -le 1) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                $bytes = $body.GetILAsByteArray()
                # Large methods calling many sub-saves
                if ($bytes.Length -gt 10000) {
                    Write-Host "MASTER SAVE METHOD CANDIDATE: $($m.Name) (Params: $($m.GetParameters().Length), IL Size: $($bytes.Length) bytes)"
                }
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
