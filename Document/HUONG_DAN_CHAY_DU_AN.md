# Hướng Dẫn Chạy Dự Án GuhaStore

## ✅ Trạng thái: Dự án đã sẵn sàng chạy!

Dự án đã được build thành công và sẵn sàng để chạy.

## 📋 Yêu cầu trước khi chạy

1. **MySQL Server** phải đang chạy
2. **Database `dbperfume`** đã được tạo và có dữ liệu
3. **.NET 9.0 SDK** đã được cài đặt

## 🚀 Các bước chạy dự án

### Bước 1: Kiểm tra MySQL Database

Đảm bảo MySQL server đang chạy và database đã được import:

```bash
# Kiểm tra MySQL đang chạy (Windows)
Get-Service | Where-Object {$_.Name -like "*MySQL*"}

# Hoặc kiểm tra trong MySQL
mysql -u root -p
SHOW DATABASES LIKE 'dbperfume';
```

Nếu chưa có database, import file `dbperfume.sql`:
```bash
mysql -u root -p < dbperfume.sql
```

### Bước 2: Cấu hình Connection String

Mở file `GuhaStore.Web/appsettings.json` và kiểm tra/sửa connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=dbperfume;User=root;Password=;CharSet=utf8mb4;"
  }
}
```

**Lưu ý quan trọng:**
- Nếu MySQL của bạn có mật khẩu, thay `Password=` thành `Password=mat_khau_cua_ban`
- Nếu MySQL chạy trên port khác (mặc định 3306), thêm `Port=3306;` vào connection string

### Bước 3: Chạy ứng dụng

#### Cách 1: Sử dụng Script PowerShell (Khuyến nghị - Tự động dừng process cũ)

Mở PowerShell tại thư mục dự án và chạy:

```powershell
.\run-app.ps1
```

Script này sẽ:
- Tự động dừng process cũ (nếu có)
- Build dự án
- Chạy ứng dụng

#### Cách 2: Sử dụng Command Line thủ công

Mở PowerShell hoặc Command Prompt tại thư mục dự án:

```powershell
# Di chuyển vào thư mục Web
cd GuhaStore.Web

# Chạy ứng dụng
dotnet run
```

Sau khi chạy, bạn sẽ thấy thông báo:
```
Now listening on: http://localhost:5115
Now listening on: https://localhost:7261
```

**Lưu ý**: Nếu gặp lỗi "File is locked", chạy script `.\stop-app.ps1` để dừng process cũ trước.

#### Cách 2: Sử dụng Visual Studio

1. Mở file `GuhaStore.sln` trong Visual Studio 2022
2. Chọn project `GuhaStore.Web` làm Startup Project (click chuột phải → Set as Startup Project)
3. Nhấn **F5** hoặc click nút **Run** (▶️)

### Bước 4: Truy cập ứng dụng

Mở trình duyệt và truy cập:

- **Trang chủ**: http://localhost:5115
- **HTTPS**: https://localhost:7261 (có thể cần chấp nhận certificate)

## 🔐 Đăng nhập

### Tạo tài khoản mới

1. Truy cập: http://localhost:5115/Account/Register
2. Điền thông tin và đăng ký
3. Tài khoản mới sẽ có quyền **Customer** (AccountType = 0)

### Đăng nhập Admin

**Lưu ý**: Tài khoản admin trong database hiện tại sử dụng MD5 hash (từ PHP). 
Bạn cần tạo tài khoản admin mới hoặc reset mật khẩu:

1. Tạo tài khoản mới qua trang đăng ký
2. Sau đó vào database và cập nhật:
   ```sql
   UPDATE account 
   SET account_type = 2, account_status = 0 
   WHERE account_name = 'ten_tai_khoan_cua_ban';
   ```

## 📁 Cấu trúc URL

- **Trang chủ**: `/` hoặc `/Home`
- **Sản phẩm**: `/Products`
- **Chi tiết sản phẩm**: `/Products/Details/{id}`
- **Giỏ hàng**: `/Cart`
- **Thanh toán**: `/Checkout`
- **Đăng nhập**: `/Account/Login`
- **Đăng ký**: `/Account/Register`
- **Tài khoản**: `/Account/MyAccount`
- **Lịch sử đơn hàng**: `/Account/OrderHistory`
- **Blog**: `/Blog`
- **Admin Panel**: `/Admin` (cần đăng nhập với quyền Staff hoặc Admin)

## ⚠️ Xử lý lỗi thường gặp

### Lỗi 1: "Unable to connect to any of the specified MySQL hosts"

**Nguyên nhân**: MySQL server chưa chạy hoặc connection string sai

**Giải pháp**:
```powershell
# Kiểm tra MySQL service
Get-Service | Where-Object {$_.Name -like "*MySQL*"}

# Khởi động MySQL nếu chưa chạy
Start-Service MySQL80  # Thay MySQL80 bằng tên service của bạn
```

### Lỗi 2: "Access denied for user 'root'@'localhost'"

**Nguyên nhân**: Mật khẩu MySQL sai hoặc user không có quyền

**Giải pháp**: 
- Kiểm tra lại mật khẩu trong `appsettings.json`
- Hoặc tạo user mới với quyền đầy đủ:
  ```sql
  CREATE USER 'guhastore'@'localhost' IDENTIFIED BY 'password';
  GRANT ALL PRIVILEGES ON dbperfume.* TO 'guhastore'@'localhost';
  FLUSH PRIVILEGES;
  ```
- Sau đó cập nhật connection string với user mới

### Lỗi 3: "Unknown database 'dbperfume'"

**Nguyên nhân**: Database chưa được tạo

**Giải pháp**:
```bash
# Import database
mysql -u root -p < dbperfume.sql

# Hoặc tạo database thủ công
mysql -u root -p
CREATE DATABASE dbperfume CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE dbperfume;
SOURCE dbperfume.sql;
```

### Lỗi 4: "File is locked by another process"

**Nguyên nhân**: Ứng dụng đang chạy ở background

**Giải pháp**:
```powershell
# Cách 1: Sử dụng script có sẵn (Khuyến nghị)
.\stop-app.ps1

# Cách 2: Dừng thủ công
Get-Process | Where-Object {$_.ProcessName -like "*GuhaStore*" -or $_.ProcessName -like "*dotnet*"} | Stop-Process -Force

# Cách 3: Đóng cửa sổ terminal đang chạy dotnet run
```

### Lỗi 5: "UseMySQL method not found"

**Nguyên nhân**: Package chưa được restore

**Giải pháp**:
```bash
dotnet restore
dotnet build
```

## 🧪 Kiểm tra ứng dụng hoạt động

Sau khi chạy thành công, kiểm tra các chức năng:

1. ✅ **Trang chủ**: Hiển thị sản phẩm, danh mục, thương hiệu
2. ✅ **Xem sản phẩm**: Click vào sản phẩm để xem chi tiết
3. ✅ **Thêm vào giỏ hàng**: Thêm sản phẩm vào giỏ
4. ✅ **Đăng ký/Đăng nhập**: Tạo tài khoản mới
5. ✅ **Thanh toán**: Tạo đơn hàng COD
6. ✅ **Xem đơn hàng**: Kiểm tra lịch sử đơn hàng

## 📝 Ghi chú quan trọng

1. **Mật khẩu**: Ứng dụng sử dụng BCrypt để hash mật khẩu (không phải MD5 như PHP)
2. **Session**: Giỏ hàng được lưu trong session, sẽ mất khi đóng trình duyệt
3. **Upload ảnh**: Thư mục upload nằm tại `wwwroot/uploads/`
4. **Email**: Email service đã được cấu hình nhưng cần cấu hình SMTP trong `appsettings.json` để gửi email thực tế

## 🎯 Bước tiếp theo

Sau khi chạy thành công, bạn có thể:
- Thêm sản phẩm mới qua Admin Panel
- Quản lý đơn hàng
- Quản lý kho hàng
- Viết bài blog
- Tùy chỉnh giao diện

Chúc bạn thành công! 🎉

