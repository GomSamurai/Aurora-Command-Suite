import subprocess

ps_script = """
$asm = [System.Reflection.Assembly]::LoadFile('C:\\VSCODE\\Aurora271Full\\Aurora.exe')
$form1 = $asm.GetType('Aurora.Form1')
if ($form1 -eq $null) {
    $form1 = $asm.GetTypes() | Where-Object { $_.Name -eq 'Form1' }
}

Write-Host "Form1 Full Name: "$form1.FullName
Write-Host "--- Methods in Form1 ---"
foreach ($m in $form1.GetMethods([System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static|DeclaredOnly')) {
    if ($m.Name -like '*Save*' -or $m.Name -like '*DB*' -or $m.Name -like '*Click*' -or $m.Name -like '*Pulse*' -or $m.Name -like '*Time*') {
        Write-Host "$($m.Name) (Params: $($m.GetParameters().Length))"
    }
}

Write-Host "\n--- Fields/Controls in Form1 ---"
foreach ($f in $form1.GetFields([System.Reflection.BindingFlags]'Public,NonPublic,Instance')) {
    if ($f.Name -like '*Save*' -or $f.Name -like '*btn*') {
        Write-Host "$($f.Name) ($($f.FieldType.Name))"
    }
}
"""

res = subprocess.run(["powershell", "-Command", ps_script], capture_output=True, text=True)
print(res.stdout)
