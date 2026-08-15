$asm = [System.Reflection.Assembly]::LoadFile("c:\VSCODE\Aurora271Full\Aurora.exe")
foreach ($t in $asm.GetTypes()) {
    $methods = $t.GetMethods("NonPublic,Public,Instance,Static")
    foreach ($m in $methods) {
        if ($m.Name -like "*Fleet*" -or $m.Name -like "*Ship*" -or $m.Name -like "*Save*" -or $m.Name -like "*Load*" -or $m.Name -like "*Refresh*") {
            if ($t.Name.Length -le 4) {
                Write-Host "Class:" $t.Name "Method:" $m.Name
            }
        }
    }
}
