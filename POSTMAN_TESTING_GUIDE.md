# 🧪 HƯỚNG DẪN TEST SINGLETON PATTERN VỚI POSTMAN

## 📦 CÀI ĐẶT

### 1. Import Postman Collection
1. Mở **Postman**
2. Click **Import** (góc trên bên trái)
3. Chọn file `SingletonPattern.postman_collection.json`
4. Collection sẽ xuất hiện với tên **"Singleton Pattern - Logger Service Demo"**

### 2. Tạo Environment (Tùy chọn)
1. Click **Environments** → **Create Environment**
2. Tên: `Singleton Pattern Local`
3. Thêm variable:
   - **Variable:** `baseUrl`
   - **Initial Value:** `http://localhost:5000`
   - **Current Value:** `http://localhost:5000`
4. Click **Save**
5. Chọn environment vừa tạo từ dropdown (góc trên bên phải)

### 3. Chạy API
```bash
cd c:\Users\Admin\Documents\School\MTK-TH
dotnet run
```

Đợi cho đến khi thấy: `Now listening on: http://localhost:5000`

---

## 🎯 CÁCH TEST

### 🔹 **Option 1: Chạy từng request một (Recommended để học)**

Chạy các request theo thứ tự từ 1 → 14:

#### **Request 1: Health Check**
- ✅ Lấy `loggerInstanceId` đầu tiên
- ✅ Lưu vào environment variable để so sánh sau

#### **Request 2-8: Gọi APIs từ nhiều Controllers**
- ✅ UserController: Get users, Get by ID, Create user
- ✅ ProductController: Get products, Search
- ✅ Mỗi request kiểm tra `loggerInstanceId` **GIỐNG NHAU**

#### **Request 9: VERIFY SINGLETON** ⭐
- ✅ Endpoint đặc biệt để kiểm tra Singleton
- ✅ Tạo 3 instances và so sánh IDs
- ✅ Console output sẽ hiện:
  ```
  🎉 SINGLETON PATTERN WORKS PERFECTLY!
  ✅ All IDs are IDENTICAL!
  ```

#### **Request 10: Get ALL Logs** 📊
- ✅ Xem TẤT CẢ logs từ mọi Controllers
- ✅ Logs từ UserController, ProductController đều có trong 1 instance
- ✅ Console breakdown: Số logs từ mỗi controller

#### **Request 11-14: Statistics & Filtering**
- ✅ Thống kê logs (INFO, WARNING, ERROR)
- ✅ Lọc logs theo level

---

### 🔹 **Option 2: Chạy toàn bộ Collection (Collection Runner)**

1. Click **Collection Runner** (hoặc ⌘/Ctrl + Alt + R)
2. Chọn collection **"Singleton Pattern - Logger Service Demo"**
3. Click **Run Singleton Pattern - Logger Service Demo**
4. Xem kết quả:
   - ✅ Tất cả tests pass (màu xanh)
   - ✅ Console output chi tiết

---

## 📊 KẾT QUẢ MONG ĐỢI

### ✅ **Request 1-8: All responses có cùng `loggerInstanceId`**

**Response từ UserController:**
```json
{
  "success": true,
  "data": [...],
  "loggerInstanceId": "12345678"
}
```

**Response từ ProductController:**
```json
{
  "success": true,
  "data": [...],
  "loggerInstanceId": "12345678"  // ← CÙNG ID!
}
```

---

### ✅ **Request 9: Verify Singleton Response**

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

**Postman Test Console:**
```
🎉 ========================================
✅ SINGLETON PATTERN WORKS PERFECTLY!
✅ Instance 1 ID: 12345678
✅ Instance 2 ID: 12345678
✅ Instance 3 ID: 12345678
✅ Current ID: 12345678
✅ Saved ID: 12345678
🎯 All IDs are IDENTICAL!
========================================
```

---

### ✅ **Request 10: Get All Logs Response**

```json
{
  "success": true,
  "totalLogs": 45,
  "data": [
    {
      "timestamp": "2026-01-12T10:30:15",
      "level": "INFO",
      "message": "UserController được khởi tạo",
      "source": "UserController"
    },
    {
      "timestamp": "2026-01-12T10:30:16",
      "level": "INFO",
      "message": "Đang lấy danh sách users",
      "source": "UserController.GetUsers"
    },
    {
      "timestamp": "2026-01-12T10:30:20",
      "level": "INFO",
      "message": "ProductController được khởi tạo",
      "source": "ProductController"
    },
    {
      "timestamp": "2026-01-12T10:30:21",
      "level": "INFO",
      "message": "Đang lấy danh sách sản phẩm",
      "source": "ProductController.GetProducts"
    }
    // ... more logs from different controllers
  ],
  "loggerInstanceId": "12345678",
  "message": "Tất cả logs từ cùng một Singleton instance"
}
```

**Postman Test Console:**
```
📊 ========================================
📝 TOTAL LOGS ACCUMULATED: 45
✅ Logger Instance ID: 12345678

📋 Log entries from different controllers:
   👤 UserController logs: 12
   📦 ProductController logs: 8
   📊 LogController logs: 15
   🔧 Other logs (Program, etc): 10

🎯 All logs from different controllers are in
   the SAME Singleton Logger instance!
========================================
```

---

### ✅ **Request 11: Log Statistics**

```json
{
  "success": true,
  "data": {
    "totalLogs": 45,
    "infoLogs": 38,
    "warningLogs": 4,
    "errorLogs": 3,
    "loggerInstanceId": "12345678"
  },
  "message": "Thống kê từ Singleton Logger instance"
}
```

**Postman Test Console:**
```
📈 ========================================
📊 LOG STATISTICS:
   Total Logs: 45
   ✅ INFO Logs: 38
   ⚠️  WARNING Logs: 4
   ❌ ERROR Logs: 3
   🔑 Logger Instance: 12345678
========================================
```

---

## 🎯 ĐIỂM CHỨNG MINH SINGLETON HOẠT ĐỘNG

### 1️⃣ **Cùng Instance ID**
- Tất cả Controllers trả về **cùng một `loggerInstanceId`**
- Postman tests tự động so sánh và verify

### 2️⃣ **Logs Tích Lũy**
- UserController ghi logs → Xem được trong LogController
- ProductController ghi logs → Vẫn trong cùng Logger
- Logs từ nhiều nguồn tích lũy trong **1 nơi duy nhất**

### 3️⃣ **Verify Endpoint Confirms**
- Tạo 3 instances trong code
- So sánh IDs
- Tất cả đều **GIỐNG NHAU**

### 4️⃣ **Statistics Consolidated**
- Thống kê logs từ **TẤT CẢ** controllers
- Chỉ có **1 nguồn dữ liệu** duy nhất

---

## 📝 AUTOMATED TESTS TRONG POSTMAN

Mỗi request có tests tự động:

### ✅ **Tests kiểm tra:**
1. **Status Code** - Response đúng (200, 201, 404, etc.)
2. **Logger Instance ID** - So sánh với ID đã lưu
3. **Data Validation** - Response có đủ fields
4. **Singleton Verification** - IDs phải giống nhau
5. **Console Logging** - Output chi tiết để debug

### 🎨 **Console Output:**
```javascript
// Example test script
pm.test("UserController uses SAME Singleton Instance", function () {
    var jsonData = pm.response.json();
    var savedInstanceId = pm.environment.get("loggerInstanceId");
    
    pm.expect(jsonData.loggerInstanceId).to.eql(savedInstanceId);
    console.log("✅ UserController Instance ID:", jsonData.loggerInstanceId);
    console.log("✅ MATCH with saved ID:", savedInstanceId);
});
```

---

## 🔧 TROUBLESHOOTING

### ❌ **Tests fail với "Connection refused"**
**Giải pháp:**
- Kiểm tra API đang chạy: `dotnet run`
- Kiểm tra URL: `http://localhost:5000`
- Thử mở trình duyệt: http://localhost:5000

### ❌ **Instance IDs khác nhau**
**Có thể do:**
- API bị restart giữa chừng (Logger bị tạo lại)
- Chạy multiple instances của API
**Giải pháp:**
- Stop API → Start lại → Chạy lại tests từ đầu

### ❌ **No logs found**
**Giải pháp:**
- Chạy requests 1-8 trước
- Request 10 (Get All Logs) phải chạy sau

---

## 🎓 BÀI TẬP MỞ RỘNG

### 1️⃣ **Thêm Custom Tests**
Thêm test script kiểm tra số lượng logs tăng dần:
```javascript
pm.test("Log count increases", function () {
    var jsonData = pm.response.json();
    var previousCount = pm.environment.get("logCount") || 0;
    
    pm.expect(jsonData.totalLogs).to.be.above(previousCount);
    pm.environment.set("logCount", jsonData.totalLogs);
});
```

### 2️⃣ **Concurrent Requests Test**
- Chạy Collection Runner với **2-3 iterations**
- Verify logs accumulate across iterations

### 3️⃣ **Performance Test**
- Chạy 100 requests
- Verify Singleton không leak memory
- Check response times

---

## 🎉 KẾT LUẬN

Collection này chứng minh:
- ✅ **Singleton Pattern hoạt động đúng**
- ✅ **Thread-safe** (nhiều requests đồng thời)
- ✅ **Data consistency** (logs từ mọi nơi vào 1 instance)
- ✅ **Memory efficient** (chỉ 1 instance duy nhất)

**Happy Testing! 🚀**
