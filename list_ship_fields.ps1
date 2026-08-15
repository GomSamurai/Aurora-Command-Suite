$asm = [System.Reflection.Assembly]::LoadFile("c:\VSCODE\Aurora271Full\Aurora.exe")
foreach ($t in $asm.GetTypes()) {
    $fields = $t.GetFields("NonPublic,Public,Instance")
    foreach ($f in $fields) {
        if ($f.FieldType.Name -eq "Ship" -or $f.FieldType.Name -eq "Fleet" -or $f.Name -like "*Ship*" -or $f.Name -like "*Fleet*") {
            if ($t.Name.Length -le 4) {
                Write-Host "Type:" $t.Name "Field:" $f.Name "FieldType:" $f.FieldType.Name
            }
        }
    }
}
