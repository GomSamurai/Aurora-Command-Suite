$asm = [System.Reflection.Assembly]::LoadFile("c:\VSCODE\Aurora271Full\Aurora.exe")
$types = $asm.GetTypes()
Write-Host "Total Types in Aurora.exe:" $types.Count
foreach ($t in $types) {
    if ($t.IsPublic -or $t.Name.Length -gt 2) {
        Write-Host "Type:" $t.FullName
    }
}
