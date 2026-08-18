import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\System.Data.SQLite.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                $il = $body.GetILAsByteArray()
                # Check for calls or news
                if ($il -ne $null -and $il.Length -gt 0) {
                    foreach ($local in $body.LocalVariables) {
                        if ($local.LocalType.FullName -like '*SQLite*') {
                            Write-Host "FOUND: $($t.FullName)::$($m.Name) has local $($local.LocalType.Name)"
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
