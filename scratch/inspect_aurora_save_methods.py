import subprocess

ps_script = """
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\0Harmony.dll')
[System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Newtonsoft.Json.dll')
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')

Write-Host "--- Searching for Save / Database Write methods in Aurora.exe ---"
foreach ($t in $asm.GetTypes()) {
    $methods = $t.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')
    foreach ($m in $methods) {
        $name = $m.Name
        if ($name -like '*Save*' -or $name -like '*Update*' -or $name -like '*DB*' -or $name -like '*Write*' -or $name -like '*Flush*') {
            Write-Host "TYPE: $($t.FullName) -> METHOD: $($m.Name) (Params: $($m.GetParameters().Length))"
        }
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
lines = res.stdout.splitlines()
print(f"Total matching methods: {len(lines)}")
for l in lines[:100]:
    print(l)
