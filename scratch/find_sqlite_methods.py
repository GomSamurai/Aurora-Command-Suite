import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

foreach ($t in $asm.GetTypes()) {
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')) {
        try {
            $body = $m.GetMethodBody()
            if ($body -ne $null) {
                # Look for methods that touch SQLite or database
                $localTypes = $body.LocalVariables | ForEach-Object { $_.LocalType.Name }
                if ($localTypes -contains 'SQLiteConnection' -or $localTypes -contains 'SqliteConnection' -or $localTypes -contains 'DbConnection') {
                    Write-Host "METHOD WITH DB LOCAL: $($t.FullName)::$($m.Name)"
                }
            }
        } catch {}
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
