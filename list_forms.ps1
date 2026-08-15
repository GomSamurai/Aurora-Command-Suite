$asm = [System.Reflection.Assembly]::LoadFile("c:\VSCODE\Aurora271Full\Aurora.exe")
$candidates = @("f5", "hf", "gu", "kn", "ko", "bd")
foreach ($name in $candidates) {
    $t = $asm.GetType($name)
    if ($t) {
        $timers = $t.GetFields("NonPublic,Public,Instance") | Where-Object { $_.FieldType.Name -eq "Timer" }
        Write-Host "Form:" $name "Timers:" $timers.Count
    }
}
