# Script để chạy ứng dụng GuhaStore
Write-Host "Đang kiểm tra và dừng process cũ (nếu có)..." -ForegroundColor Yellow

# Dừng process cũ
$processes = Get-Process | Where-Object {
    $_.ProcessName -like "*GuhaStore*" -or 
    ($_.ProcessName -eq "dotnet" -and $_.Path -like "*GuhaStore*")
}

if ($processes) {
    foreach ($proc in $processes) {
        Write-Host "Dừng process: $($proc.ProcessName) (ID: $($proc.Id))" -ForegroundColor Cyan
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 2
}

Write-Host "`nĐang build dự án..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild thành công! Đang chạy ứng dụng..." -ForegroundColor Green
    Write-Host "Ứng dụng sẽ chạy tại: http://localhost:5115" -ForegroundColor Cyan
    Write-Host "Nhấn Ctrl+C để dừng ứng dụng`n" -ForegroundColor Yellow
    
    cd GuhaStore.Web
    dotnet run
} else {
    Write-Host "`nBuild thất bại! Vui lòng kiểm tra lỗi ở trên." -ForegroundColor Red
    exit 1
}

