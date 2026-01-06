# Script để dừng ứng dụng GuhaStore đang chạy
Write-Host "Đang tìm và dừng các process GuhaStore..." -ForegroundColor Yellow

$processes = Get-Process | Where-Object {
    $_.ProcessName -like "*GuhaStore*" -or 
    ($_.ProcessName -eq "dotnet" -and $_.Path -like "*GuhaStore*")
}

if ($processes) {
    foreach ($proc in $processes) {
        Write-Host "Dừng process: $($proc.ProcessName) (ID: $($proc.Id))" -ForegroundColor Cyan
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "Đã dừng tất cả process!" -ForegroundColor Green
    Start-Sleep -Seconds 2
} else {
    Write-Host "Không tìm thấy process nào đang chạy." -ForegroundColor Green
}

