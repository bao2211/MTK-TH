# LAB THỰC HÀNH - SINGLETON PATTERN
## Môn: Design Pattern - ASP.NET Core Web API

### 📋 MỤC TIÊU
Xây dựng **Logger Service** áp dụng **Singleton Pattern** - đảm bảo chỉ có **một instance duy nhất** trong toàn bộ ứng dụng và sử dụng trong nhiều Controller.

---

## 🎯 SINGLETON PATTERN LÀ GÌ?

**Singleton Pattern** là một Creational Design Pattern đảm bảo:
- ✅ Một class chỉ có **duy nhất một instance**
- ✅ Cung cấp một **điểm truy cập toàn cục** đến instance đó
- ✅ Instance được tạo **lazy** (chỉ khi cần thiết)
- ✅ **Thread-safe** (an toàn trong môi trường đa luồng)

---

## 🏗️ CẤU TRÚC DỰ ÁN

```
MTK-TH/
├── Controllers/
│   ├── UserController.cs       # Quản lý Users
│   ├── ProductController.cs    # Quản lý Products
│   └── LogController.cs        # Xem và quản lý logs
├── Services/
│   └── LoggerService.cs        # Singleton Logger Service
├── Program.cs                   # Cấu hình ứng dụng
├── appsettings.json
└── SingletonPattern.csproj
```

---

## 📝 CHI TIẾT TRIỂN KHAI

### 1. **LoggerService.cs** - Singleton Logger

**Đặc điểm quan trọng:**
- ✅ `private static LoggerService? _instance` - Instance tĩnh duy nhất
- ✅ `private LoggerService()` - Constructor private
- ✅ `public static LoggerService Instance` - Điểm truy cập toàn cục
- ✅ **Double-Check Locking** - Thread-safe pattern
- ✅ **Sealed class** - Ngăn kế thừa

**Chức năng:**
- `LogInfo()` - Ghi log INFO
- `LogWarning()` - Ghi log WARNING
- `LogError()` - Ghi log ERROR
- `GetAllLogs()` - Lấy tất cả logs
- `GetLogsByLevel()` - Lấy logs theo level
- `ClearLogs()` - Xóa logs
- `GetInstanceId()` - Lấy ID để kiểm tra Singleton

### 2. **Controllers**

#### **UserController.cs**
- `GET /api/user` - Lấy danh sách users
- `GET /api/user/{id}` - Lấy user theo ID
- `POST /api/user` - Tạo user mới

#### **ProductController.cs**
- `GET /api/product` - Lấy danh sách sản phẩm
- `GET /api/product/search?keyword=...` - Tìm kiếm sản phẩm
- `DELETE /api/product/{id}` - Xóa sản phẩm

#### **LogController.cs**
- `GET /api/log` - Lấy tất cả logs
- `GET /api/log/level/{level}` - Lấy logs theo level (INFO/WARNING/ERROR)
- `GET /api/log/stats` - Thống kê logs
- `DELETE /api/log` - Xóa tất cả logs
- `GET /api/log/verify-singleton` - **Kiểm tra Singleton hoạt động đúng**

---

## 🚀 HƯỚNG DẪN CHẠY ỨNG DỤNG

### **1. Restore và Build**
```bash
dotnet restore
dotnet build
```

### **2. Chạy ứng dụng**
```bash
dotnet run
```

### **3. Truy cập Swagger UI**
Mở trình duyệt tại: **http://localhost:5000**

---

## 🧪 KIỂM THỬ SINGLETON PATTERN

### **Test 1: Gọi các API từ Controllers khác nhau**

```bash
# 1. Gọi UserController
curl http://localhost:5000/api/user

# 2. Gọi ProductController
curl http://localhost:5000/api/product

# 3. Xem logs - logs từ cả 2 controllers trên sẽ xuất hiện
curl http://localhost:5000/api/log
```

**Kết quả mong đợi:** 
- Tất cả logs từ `UserController` và `ProductController` đều được lưu trong cùng một instance
- `LoggerInstanceId` trong response của cả 3 API sẽ **giống nhau**

### **Test 2: Kiểm tra Singleton trực tiếp**

```bash
curl http://localhost:5000/api/log/verify-singleton
```

**Response:**
```json
{
  "success": true,
  "isSingleton": true,
  "instance1Id": "12345678",
  "instance2Id": "12345678",
  "instance3Id": "12345678",
  "currentInstanceId": "12345678",
  "message": "✓ Tất cả đều trỏ đến cùng một instance - Singleton hoạt động đúng!"
}
```

### **Test 3: Xem thống kê logs**

```bash
curl http://localhost:5000/api/log/stats
```

---

## 💡 ĐIỂM QUAN TRỌNG CẦN NHỚ

### ✅ **Ưu điểm của Singleton**
1. **Tiết kiệm tài nguyên** - Chỉ tạo một instance duy nhất
2. **Truy cập toàn cục** - Dễ dàng truy cập từ mọi nơi
3. **Đồng bộ dữ liệu** - Tất cả sử dụng cùng một nguồn dữ liệu

### ⚠️ **Lưu ý khi sử dụng Singleton**
1. **Thread-safety** - Phải đảm bảo an toàn đa luồng (đã implement với lock)
2. **Testing khó khăn** - Singleton khó mock trong unit test
3. **Global state** - Có thể gây khó khăn trong quản lý state

### 🔧 **Kỹ thuật implement**
- **Double-Check Locking** - Kiểm tra null 2 lần để tối ưu hiệu năng
- **Sealed class** - Ngăn kế thừa để bảo vệ pattern
- **Private constructor** - Ngăn tạo instance từ bên ngoài
- **Thread-safe operations** - Sử dụng `lock` cho các thao tác đọc/ghi

---

## 📊 KẾT QUẢ MONG ĐỢI

Khi chạy ứng dụng, bạn sẽ thấy:

1. **Console output** hiển thị:
   - Thông báo khởi tạo Logger instance (chỉ 1 lần)
   - Các log messages với màu sắc phù hợp
   - Instance ID giống nhau cho mọi request

2. **Swagger UI** cho phép:
   - Test các API endpoints
   - Xem response với LoggerInstanceId
   - Kiểm tra logs được tích lũy từ nhiều controllers

3. **Verification endpoint** xác nhận:
   - Tất cả instance IDs đều giống nhau
   - Singleton pattern hoạt động đúng

---

## 📚 BÀI TẬP MỞ RỘNG

1. **Thêm log vào file** - Ghi logs ra file thay vì chỉ console
2. **Thêm log levels mới** - DEBUG, FATAL, TRACE
3. **Filter logs** - Lọc logs theo thời gian, source
4. **Thread-safety test** - Viết test đa luồng để kiểm tra thread-safety
5. **Dependency Injection** - So sánh với việc đăng ký Singleton qua DI container

---

## 🎓 KẾT LUẬN

Bài lab này minh họa:
- ✅ Cách implement **Singleton Pattern** đúng chuẩn
- ✅ **Thread-safe** với Double-Check Locking
- ✅ Sử dụng Singleton trong **ASP.NET Core Web API**
- ✅ Kiểm tra và xác nhận pattern hoạt động đúng
- ✅ Ứng dụng thực tế với Logger Service

**Happy Coding! 🚀**
