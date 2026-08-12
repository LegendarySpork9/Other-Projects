$publishDir = "$PSScriptRoot\LiveScreensaver\bin\Publish"
$exePath = "$publishDir\LiveScreensaver.exe"
$scrPath = "$env:WINDIR\System32\LiveScreensaver.scr"

if (-not (Test-Path $exePath)) {
    Write-Host "LiveScreensaver.exe not found. Run 'dotnet publish' first." -ForegroundColor Red
    exit 1
}

Write-Host "Copying LiveScreensaver.exe -> $scrPath" -ForegroundColor Cyan
Copy-Item $exePath $scrPath -Force

$dlls = Get-ChildItem $publishDir -Filter "*.dll"
foreach ($dll in $dlls) {
    $dest = "$env:WINDIR\System32\$($dll.Name)"
    Write-Host "Copying $($dll.Name) -> $dest" -ForegroundColor Cyan
    Copy-Item $dll.FullName $dest -Force
}

Write-Host "`nInstalled! Open Settings > Personalization > Lock screen > Screen saver and select 'LiveScreensaver'." -ForegroundColor Green
Write-Host "Right-click > Settings to configure your video folder." -ForegroundColor Green
