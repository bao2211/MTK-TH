# 📦 POSTMAN TEST COLLECTIONS
## Design Patterns Lab - ASP.NET Core Web API

Thư mục này chứa các Postman Collections để test các Design Patterns được implement trong project.

---

## 📋 **DANH SÁCH COLLECTIONS**

### 1️⃣ **Singleton.postman_collection.json**
- **Pattern:** Singleton Pattern
- **Mục đích:** Test Logger Service - chứng minh chỉ có 1 instance duy nhất
- **Số requests:** 14 requests
- **Highlights:**
  - ✅ Verify cùng Logger Instance ID across Controllers
  - ✅ Xem logs tích lũy từ nhiều Controllers
  - ✅ Statistics và filtering logs
  - ✅ Automated tests với JavaScript

**Key endpoints:**
- `GET /health` - Health check
- `GET /api/user` - UserController
- `GET /api/product` - ProductController
- `GET /api/log/verify-singleton` - **Verify Singleton**
- `GET /api/log` - Xem tất cả logs

---

### 2️⃣ **FactoryMethod.postman_collection.json**
- **Pattern:** Factory Method Pattern
- **Mục đích:** Test Payment API - demo Factory tạo các payment methods khác nhau
- **Số requests:** 10 requests
- **Highlights:**
  - ✅ Test 3 payment methods: Cash, PayPal, VNPay
  - ✅ Tính phí giao dịch cho từng method
  - ✅ Demo Factory Pattern
  - ✅ Test validation và error handling

**Key endpoints:**
- `GET /api/payment/methods` - Danh sách payment methods
- `POST /api/payment/process` - Xử lý thanh toán
- `GET /api/payment/calculate-fee` - Tính phí
- `GET /api/payment/demo-factory` - **Demo Factory Pattern**

---

## 🚀 **HƯỚNG DẪN SỬ DỤNG**

### **Bước 1: Import vào Postman**

1. Mở **Postman**
2. Click **Import** (góc trên bên trái)
3. Click **Choose Files** hoặc kéo thả file
4. Chọn file collection từ thư mục `Postman/`
5. Collection sẽ xuất hiện trong sidebar

### **Bước 2: Setup Environment (Optional)**

Tạo environment cho thuận tiện:

1. Click **Environments** → **Create Environment**
2. Tên: `Design Patterns Local`
3. Thêm variable:
   - **Variable:** `baseUrl`
   - **Initial Value:** `http://localhost:5000`
   - **Current Value:** `http://localhost:5000`
4. Click **Save**
5. Chọn environment từ dropdown (góc trên bên phải)

### **Bước 3: Chạy API**

```powershell
# Từ thư mục project
cd c:\Users\Admin\Documents\School\MTK-TH
dotnet run
```

Đợi message: `Now listening on: http://localhost:5000`

### **Bước 4: Test Collections**

#### **Option A: Chạy từng request (Recommended)**
- Click vào request
- Click **Send**
- Xem Response và Test Results
- Xem Console output (Ctrl + Alt + C)

#### **Option B: Chạy toàn bộ Collection**
- Click **Collection Runner** (⌘/Ctrl + Alt + R)
- Chọn collection
- Click **Run**
- Xem summary results

---

## 📊 **AUTOMATED TESTS**

Mỗi collection có **automated tests** được viết bằng JavaScript:

### **Test checks:**
- ✅ **Status codes** - 200, 201, 400, 404, 500
- ✅ **Response structure** - Validate JSON fields
- ✅ **Business logic** - Singleton verification, Fee calculation
- ✅ **Data consistency** - Instance IDs, logs accumulation
- ✅ **Error handling** - Invalid inputs, unsupported methods

### **Console output:**
- 📊 Statistics và summaries
- ✅ Success indicators
- ❌ Error details
- 🎯 Pattern-specific insights

---

## 🎯 **SINGLETON COLLECTION - KEY TESTS**

### **Test 1-8: Multiple Controllers**
```javascript
// Auto-verify: All controllers use SAME Logger instance
pm.expect(jsonData.loggerInstanceId).to.eql(savedInstanceId);
```

### **Test 9: Verify Singleton ⭐**
```javascript
// Verify: instance1 === instance2 === instance3
pm.expect(jsonData.isSingleton).to.be.true;
```

### **Test 10: Accumulated Logs 📊**
```javascript
// Show logs from ALL controllers in ONE place
console.log("UserController logs:", userControllerLogs);
console.log("ProductController logs:", productControllerLogs);
```

---

## 🏭 **FACTORY METHOD COLLECTION - KEY TESTS**

### **Test 2-4: Different Payment Methods**
```javascript
// Factory creates different classes based on input
pm.test("Factory created CashPayment", function() {
    pm.expect(jsonData.data.paymentType).to.equal('CASH');
});
```

### **Test 5-7: Fee Calculation**
```javascript
// Each payment method has different fee logic
console.log(`Fee: ${fee} (${percentage}%)`);
```

### **Test 8: Demo Factory Pattern 🎯**
```javascript
// Show how Factory creates multiple products
console.log("Factory Type:", jsonData.data.factoryType);
console.log("Products created:", paymentMethods);
```

---

## 📈 **EXPECTED RESULTS**

### **Singleton Collection:**

✅ **14/14 tests pass**
```
✅ All requests return same loggerInstanceId
✅ Singleton verification: isSingleton = true
✅ Logs from different controllers accumulated
✅ Statistics show consolidated data
```

**Console highlights:**
```
🎉 SINGLETON PATTERN WORKS PERFECTLY!
✅ All IDs are IDENTICAL!
📊 Total Logs: 45
   👤 UserController: 12 logs
   📦 ProductController: 8 logs
```

---

### **Factory Method Collection:**

✅ **10/10 tests pass** (7 success + 3 validation errors expected)
```
✅ Cash payment: 0% fee
✅ PayPal payment: 3.4% + $0.30 fee
✅ VNPay payment: 2% fee (max 50k)
✅ Factory demo shows 3 different classes
❌ Unsupported payment method rejected (expected)
❌ Invalid bank code rejected (expected)
```

**Console highlights:**
```
🏭 FACTORY METHOD PATTERN DEMO
📦 Products created by Factory:
  ✅ CASH - Class: CashPayment
  ✅ PAYPAL - Class: PaypalPayment
  ✅ VNPAY - Class: VNPayPayment
```

---

## 🔧 **TROUBLESHOOTING**

### ❌ **Connection Refused**
```
Error: connect ECONNREFUSED 127.0.0.1:5000
```
**Fix:** Chạy API trước: `dotnet run`

### ❌ **Tests Fail**
```
Expected instance IDs to match but got different values
```
**Fix:** API bị restart → Instance mới được tạo → Clear logs và chạy lại từ đầu

### ❌ **Environment Variables Not Found**
```
Error: baseUrl is not defined
```
**Fix:** 
- Chọn environment từ dropdown
- Hoặc thay `{{baseUrl}}` → `http://localhost:5000` trong requests

---

## 📚 **RESOURCES**

### **Documentation:**
- [Singleton Pattern README](../README.md)
- [Factory Method README](../FACTORY_METHOD_README.md)
- [Postman Testing Guide](../POSTMAN_TESTING_GUIDE.md)

### **API Documentation:**
- Swagger UI: http://localhost:5000
- Health Check: http://localhost:5000/health

---

## 🎓 **LEARNING OBJECTIVES**

Sau khi chạy tests, bạn sẽ hiểu:

### **Singleton Pattern:**
- ✅ Một class chỉ có 1 instance duy nhất
- ✅ Thread-safe implementation
- ✅ Global access point
- ✅ Use case: Logger, Config, Cache

### **Factory Method Pattern:**
- ✅ Tách logic tạo objects khỏi client code
- ✅ Factory quyết định class nào được tạo
- ✅ Dễ mở rộng thêm products mới
- ✅ Use case: Payment methods, File exporters, Notifications

---

## 💡 **TIPS**

1. **Chạy theo thứ tự** - Requests được đánh số để dễ follow
2. **Xem Console** - Output chi tiết với emoji và formatting
3. **Run Collection** - Để xem tổng quan tất cả tests
4. **Save Responses** - Có thể save làm examples
5. **Create Variations** - Duplicate requests để test cases khác

---

**Happy Testing! 🚀**

Made with ❤️ for Design Patterns Lab
