# GuhaStore - E-Commerce Application

Ứng dụng thương mại điện tử được xây dựng bằng ASP.NET Core MVC với MySQL database.

## Yêu cầu hệ thống

- .NET 9.0 SDK
- MySQL Server 8.0 trở lên
- Database `dbperfume` đã được tạo và có dữ liệu

## Cài đặt và chạy dự án

### Bước 1: Kiểm tra MySQL Database

Đảm bảo MySQL server đang chạy và database `dbperfume` đã tồn tại:

```sql
-- Kiểm tra database
SHOW DATABASES LIKE 'dbperfume';

-- Nếu chưa có, import file dbperfume.sql
mysql -u root -p < dbperfume.sql
```

### Bước 2: Cấu hình Connection String

Mở file `GuhaStore.Web/appsettings.json` và kiểm tra connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=dbperfume;User=root;Password=YOUR_PASSWORD;CharSet=utf8mb4;"
  }
}
```

**Lưu ý:** Thay `YOUR_PASSWORD` bằng mật khẩu MySQL của bạn. Nếu không có mật khẩu, để trống: `Password=;`

### Bước 3: Restore NuGet Packages

```bash
dotnet restore
```

### Bước 4: Build dự án

```bash
dotnet build
```

### Bước 5: Chạy ứng dụng

Có 2 cách để chạy:

#### Cách 1: Sử dụng dotnet CLI

```bash
cd GuhaStore.Web
dotnet run
```

Ứng dụng sẽ chạy tại:
- HTTP: http://localhost:5115
- HTTPS: https://localhost:7261

#### Cách 2: Sử dụng Visual Studio

1. Mở file `GuhaStore.sln` trong Visual Studio
2. Chọn project `GuhaStore.Web` làm startup project
3. Nhấn F5 hoặc click "Run"

### Bước 6: Truy cập ứng dụng

Mở trình duyệt và truy cập:
- **Trang chủ**: http://localhost:5115
- **Trang admin**: http://localhost:5115/Admin (cần đăng nhập với tài khoản admin)

## Tài khoản mặc định

Sau khi import database, bạn có thể đăng nhập với:
- **Admin**: Username: `Admin`, Password: `admin` (cần hash lại bằng BCrypt)
- Hoặc tạo tài khoản mới qua trang đăng ký

## Cấu trúc dự án

```
GuhaStorePHP/
├── GuhaStore.Core/          # Domain entities và interfaces
├── GuhaStore.Application/    # Business logic và services
├── GuhaStore.Infrastructure/ # Data access, repositories
├── GuhaStore.Web/           # Controllers, Views, UI
└── dbperfume.sql            # Database schema và dữ liệu
```

## Tính năng chính

- ✅ Quản lý sản phẩm (Products, Categories, Brands)
- ✅ Giỏ hàng (Shopping Cart)
- ✅ Đặt hàng COD (Cash on Delivery)
- ✅ Quản lý đơn hàng
- ✅ Quản lý tài khoản khách hàng
- ✅ Blog/Articles
- ✅ Quản lý kho (Inventory)
- ✅ Admin Panel

## Xử lý lỗi thường gặp

### Lỗi: "Unable to connect to database"

**Nguyên nhân**: MySQL server chưa chạy hoặc connection string sai

**Giải pháp**:
1. Kiểm tra MySQL service đang chạy
2. Kiểm tra connection string trong `appsettings.json`
3. Kiểm tra database `dbperfume` đã tồn tại

### Lỗi: "File is locked by another process"

**Nguyên nhân**: Ứng dụng đang chạy ở background

**Giải pháp**:
```powershell
# Tìm và dừng process
Get-Process | Where-Object {$_.ProcessName -like "*GuhaStore*"} | Stop-Process -Force
```

### Lỗi: "UseMySQL method not found"

**Nguyên nhân**: Thiếu namespace hoặc package

**Giải pháp**:
```bash
cd GuhaStore.Infrastructure
dotnet add package MySql.EntityFrameworkCore --version 9.0.0
```

## Phát triển tiếp

Để phát triển thêm tính năng, tham khảo file:
- `guhastore-complete-implementation-plan-46635e.plan.md` - Kế hoạch triển khai chi tiết

## Hỗ trợ

Nếu gặp vấn đề, kiểm tra:
1. Logs trong console khi chạy `dotnet run`
2. File `appsettings.json` để đảm bảo connection string đúng
3. Database đã được import đầy đủ

