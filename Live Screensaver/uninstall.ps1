$scrPath = "$env:WINDIR\System32\LiveScreensaver.scr"

$dlls = @(
    "D3DCompiler_47_cor3.dll",
    "PenImc_cor3.dll",
    "PresentationNative_cor3.dll",
    "vcruntime140_cor3.dll",
    "wpfgfx_cor3.dll"
)

if (Test-Path $scrPath) {
    Remove-Item $scrPath -Force -Confirm:$false
    Write-Host "Removed $scrPath" -ForegroundColor Cyan
}

foreach ($dll in $dlls) {
    $path = "$env:WINDIR\System32\$dll"
    if (Test-Path $path) {
        Remove-Item $path -Force -Confirm:$false
        Write-Host "Removed $path" -ForegroundColor Cyan
    }
}

$regPath = "HKCU:\SOFTWARE\LiveScreensaver"
if (Test-Path $regPath) {
    Remove-Item $regPath -Recurse -Force -Confirm:$false
    Write-Host "Removed registry key $regPath" -ForegroundColor Cyan
}

Write-Host "`nUninstalled." -ForegroundColor Green
