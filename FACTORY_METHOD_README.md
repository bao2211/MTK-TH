# BÀI 2 - FACTORY METHOD PATTERN
## Môn: Design Pattern - ASP.NET Core Web API

### 📋 MỤC TIÊU
Xây dựng **Payment API** hỗ trợ nhiều phương thức thanh toán (**Cash, PayPal, VNPay**) sử dụng **Factory Method Pattern**.

---

## 🎯 FACTORY METHOD PATTERN LÀ GÌ?

**Factory Method Pattern** là một Creational Design Pattern định nghĩa một **interface để tạo objects**, nhưng để **subclasses quyết định** class nào sẽ được instantiate.

### **Đặc điểm:**
- ✅ Tách việc **tạo object** khỏi **logic sử dụng**
- ✅ Giảm sự phụ thuộc vào concrete classes
- ✅ Dễ mở rộng thêm product mới (Open/Closed Principle)
- ✅ Client code không cần biết class cụ thể được tạo

### **Các thành phần:**
1. **Product** (IPaymentMethod) - Interface chung
2. **Concrete Products** (CashPayment, PaypalPayment, VNPayPayment) - Implementations
3. **Creator** (IPaymentFactory) - Factory interface
4. **Concrete Creator** (PaymentFactory) - Factory implementation

---

## 🏗️ CẤU TRÚC DỰ ÁN

```
MTK-TH/
├── Services/
│   ├── LoggerService.cs                      # Singleton (Bài 1)
│   └── PaymentFactory/
│       ├── IPaymentMethod.cs                 # Product Interface
│       ├── CashPayment.cs                    # Concrete Product 1
│       ├── PaypalPayment.cs                  # Concrete Product 2
│       ├── VNPayPayment.cs                   # Concrete Product 3
│       ├── IPaymentFactory.cs                # Creator Interface
│       └── PaymentFactory.cs                 # Concrete Creator
├── Controllers/
│   ├── PaymentController.cs                  # Payment API
│   ├── UserController.cs                     # (Bài 1)
│   ├── ProductController.cs                  # (Bài 1)
│   └── LogController.cs                      # (Bài 1)
├── Models/
│   └── PaymentModels.cs                      # Request/Response models
└── Program.cs
```

---

## 📝 CHI TIẾT TRIỂN KHAI

### 1️⃣ **IPaymentMethod.cs - Product Interface**

Định nghĩa contract chung cho tất cả payment methods:

```csharp
public interface IPaymentMethod
{
    string PaymentType { get; }
    PaymentResult ProcessPayment(decimal amount, string orderId, ...);
    bool ValidatePayment(decimal amount, ...);
    decimal GetTransactionFee(decimal amount);
}
```

**Vai trò:** Interface chung mà tất cả payment methods phải implement.

---

### 2️⃣ **Concrete Products - Payment Implementations**

#### **CashPayment.cs** - Thanh toán tiền mặt
```csharp
public class CashPayment : IPaymentMethod
{
    public string PaymentType => "CASH";
    
    public PaymentResult ProcessPayment(...)
    {
        // Logic xử lý tiền mặt
        // Không có phí giao dịch
    }
    
    public decimal GetTransactionFee(decimal amount) => 0;
}
```

**Đặc điểm:**
- ✅ Không có phí giao dịch
- ✅ Giới hạn tối đa: 100 triệu VNĐ
- ✅ Yêu cầu: OrderId, Amount

#### **PaypalPayment.cs** - Thanh toán PayPal
```csharp
public class PaypalPayment : IPaymentMethod
{
    public string PaymentType => "PAYPAL";
    
    public decimal GetTransactionFee(decimal amount)
    {
        // PayPal: 3.4% + $0.30
        return amount * 0.034m + 0.30m;
    }
}
```

**Đặc điểm:**
- ✅ Phí: 3.4% + $0.30 mỗi giao dịch
- ✅ Currency: USD
- ✅ Yêu cầu: OrderId, Amount, **PaypalEmail**

#### **VNPayPayment.cs** - Thanh toán VNPay
```csharp
public class VNPayPayment : IPaymentMethod
{
    public string PaymentType => "VNPAY";
    
    public decimal GetTransactionFee(decimal amount)
    {
        // VNPay: 2%, max 50,000 VNĐ
        var fee = amount * 0.02m;
        return Math.Min(fee, 50_000);
    }
}
```

**Đặc điểm:**
- ✅ Phí: 2% (tối đa 50,000 VNĐ)
- ✅ Số tiền tối thiểu: 10,000 VNĐ
- ✅ Yêu cầu: OrderId, Amount, **BankCode**

---

### 3️⃣ **PaymentFactory.cs - Factory Implementation**

**Core của Factory Method Pattern:**

```csharp
public class PaymentFactory : IPaymentFactory
{
    public IPaymentMethod CreatePaymentMethod(string paymentType)
    {
        return paymentType.ToUpper() switch
        {
            "CASH" => new CashPayment(),      // ← Factory quyết định
            "PAYPAL" => new PaypalPayment(),  // ← tạo class nào
            "VNPAY" => new VNPayPayment(),    // ← dựa trên input
            _ => throw new NotSupportedException(...)
        };
    }
}
```

**Vai trò:**
- ✅ **Encapsulate** logic tạo objects
- ✅ Client không cần biết concrete classes
- ✅ Dễ thêm payment method mới

---

### 4️⃣ **PaymentController.cs - Client Code**

Sử dụng Factory để xử lý payments:

```csharp
[HttpPost("process")]
public IActionResult ProcessPayment([FromBody] PaymentRequest request)
{
    // 🎯 FACTORY METHOD PATTERN
    IPaymentMethod paymentMethod = _paymentFactory.CreatePaymentMethod(request.PaymentType);
    
    // Xử lý thanh toán mà không cần biết class cụ thể
    var result = paymentMethod.ProcessPayment(request.Amount, request.OrderId, ...);
    
    return Ok(result);
}
```

**Lợi ích:**
- ✅ Controller không phụ thuộc vào concrete classes
- ✅ Dễ test (có thể mock IPaymentFactory)
- ✅ Thêm payment method mới không cần sửa controller

---

## 🚀 API ENDPOINTS

### **1. Process Payment - Xử lý thanh toán**
```http
POST /api/payment/process
Content-Type: application/json

{
  "paymentType": "CASH",
  "amount": 500000,
  "orderId": "ORD001",
  "additionalData": {
    "Cashier": "John Doe"
  }
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thanh toán thành công",
  "data": {
    "transactionId": "CASH-20260112103045-1234",
    "paymentType": "CASH",
    "amount": 500000,
    "transactionFee": 0,
    "totalAmount": 500000
  }
}
```

---

### **2. Get Payment Methods - Danh sách phương thức**
```http
GET /api/payment/methods
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "type": "CASH",
      "displayName": "Tiền mặt",
      "feeDescription": "Miễn phí",
      "requiredFields": ["OrderId", "Amount"]
    },
    {
      "type": "PAYPAL",
      "displayName": "PayPal",
      "feeDescription": "3.4% + $0.30 mỗi giao dịch",
      "requiredFields": ["OrderId", "Amount", "PaypalEmail"]
    },
    {
      "type": "VNPAY",
      "displayName": "VNPay",
      "feeDescription": "2% (tối đa 50,000 VNĐ)",
      "requiredFields": ["OrderId", "Amount", "BankCode"]
    }
  ]
}
```

---

### **3. Calculate Fee - Tính phí giao dịch**
```http
GET /api/payment/calculate-fee?paymentType=VNPAY&amount=1000000
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentType": "VNPAY",
    "amount": 1000000,
    "transactionFee": 20000,
    "totalAmount": 1020000,
    "feePercentage": 2.0
  }
}
```

---

### **4. Demo Factory - Minh họa pattern**
```http
GET /api/payment/demo-factory
```

**Response:**
```json
{
  "success": true,
  "message": "Factory Method Pattern Demo",
  "data": {
    "paymentMethods": [
      {
        "paymentType": "CASH",
        "className": "CashPayment",
        "hashCode": 12345678,
        "sampleFee": 0
      },
      {
        "paymentType": "PAYPAL",
        "className": "PaypalPayment",
        "hashCode": 87654321,
        "sampleFee": 34.3
      }
    ]
  }
}
```

---

## 🧪 TEST CASES

### **Test 1: Thanh toán tiền mặt**
```bash
curl -X POST http://localhost:5000/api/payment/process \
  -H "Content-Type: application/json" \
  -d '{
    "paymentType": "CASH",
    "amount": 500000,
    "orderId": "ORD001"
  }'
```

**Kết quả:** ✅ Transaction fee = 0

---

### **Test 2: Thanh toán PayPal**
```bash
curl -X POST http://localhost:5000/api/payment/process \
  -H "Content-Type: application/json" \
  -d '{
    "paymentType": "PAYPAL",
    "amount": 100,
    "orderId": "ORD002",
    "additionalData": {
      "PaypalEmail": "user@example.com"
    }
  }'
```

**Kết quả:** ✅ Transaction fee = $3.70 (3.4% + $0.30)

---

### **Test 3: Thanh toán VNPay**
```bash
curl -X POST http://localhost:5000/api/payment/process \
  -H "Content-Type: application/json" \
  -d '{
    "paymentType": "VNPAY",
    "amount": 1000000,
    "orderId": "ORD003",
    "additionalData": {
      "BankCode": "NCB"
    }
  }'
```

**Kết quả:** ✅ Transaction fee = 20,000 VNĐ (2%)

---

### **Test 4: Payment method không hỗ trợ**
```bash
curl -X POST http://localhost:5000/api/payment/process \
  -H "Content-Type: application/json" \
  -d '{
    "paymentType": "BITCOIN",
    "amount": 1000000,
    "orderId": "ORD004"
  }'
```

**Kết quả:** ❌ Error: "Phương thức thanh toán 'BITCOIN' không được hỗ trợ"

---

## 💡 FACTORY METHOD vs NEW OPERATOR

### **❌ Không dùng Factory (Bad)**
```csharp
// Controller code
public IActionResult ProcessPayment(PaymentRequest request)
{
    IPaymentMethod payment;
    
    // ❌ Controller phụ thuộc vào concrete classes
    if (request.PaymentType == "CASH")
        payment = new CashPayment();
    else if (request.PaymentType == "PAYPAL")
        payment = new PaypalPayment();
    else if (request.PaymentType == "VNPAY")
        payment = new VNPayPayment();
    else
        throw new Exception("Not supported");
    
    // Thêm payment mới → Phải sửa controller ❌
}
```

**Vấn đề:**
- ❌ Controller biết tất cả concrete classes
- ❌ Thêm payment mới phải sửa nhiều nơi
- ❌ Khó test và maintain

---

### **✅ Dùng Factory (Good)**
```csharp
public IActionResult ProcessPayment(PaymentRequest request)
{
    // ✅ Controller chỉ phụ thuộc vào interface
    IPaymentMethod payment = _paymentFactory.CreatePaymentMethod(request.PaymentType);
    
    // Xử lý payment
    // Thêm payment mới → Chỉ sửa factory ✅
}
```

**Lợi ích:**
- ✅ Controller không biết concrete classes
- ✅ Thêm payment mới chỉ sửa factory
- ✅ Dễ test với mock factory
- ✅ Follow SOLID principles

---

## 🔄 THÊM PAYMENT METHOD MỚI

Giả sử thêm **MoMo Payment:**

### **Bước 1: Tạo MoMoPayment.cs**
```csharp
public class MoMoPayment : IPaymentMethod
{
    public string PaymentType => "MOMO";
    
    public PaymentResult ProcessPayment(...)
    {
        // Logic MoMo
    }
    
    public decimal GetTransactionFee(decimal amount)
    {
        return amount * 0.015m; // 1.5%
    }
}
```

### **Bước 2: Cập nhật Factory**
```csharp
public IPaymentMethod CreatePaymentMethod(string paymentType)
{
    return paymentType.ToUpper() switch
    {
        "CASH" => new CashPayment(),
        "PAYPAL" => new PaypalPayment(),
        "VNPAY" => new VNPayPayment(),
        "MOMO" => new MoMoPayment(),  // ← Chỉ thêm 1 dòng!
        _ => throw new NotSupportedException(...)
    };
}
```

**Xong! Không cần sửa:**
- ✅ PaymentController
- ✅ Client code
- ✅ Existing payment methods

---

## 🎓 SO SÁNH VỚI SINGLETON

| Pattern | Singleton | Factory Method |
|---------|-----------|----------------|
| **Mục đích** | 1 instance duy nhất | Tạo nhiều objects khác nhau |
| **Khi nào dùng** | Logger, Config, Cache | Payment, Notification, Export |
| **Creation** | Tự tạo chính nó | Factory tạo cho client |
| **Instance** | Cùng 1 object | Mỗi lần tạo object mới |

**Kết hợp cả 2:**
```csharp
// LoggerService: Singleton - 1 instance duy nhất
var logger = LoggerService.Instance;

// PaymentFactory: Factory Method - Tạo nhiều payments khác nhau
var cashPayment = factory.CreatePaymentMethod("CASH");
var paypalPayment = factory.CreatePaymentMethod("PAYPAL");
```

---

## 📊 DIAGRAM

```
┌─────────────────────────────────────────────────┐
│             PaymentController                   │
│  (Client - không biết concrete classes)        │
└──────────────┬──────────────────────────────────┘
               │ uses
               ▼
      ┌────────────────┐
      │ IPaymentFactory│ ◄─────── Factory Interface
      └────────┬───────┘
               │ implements
               ▼
      ┌────────────────┐
      │ PaymentFactory │ ◄─────── Concrete Factory
      └────────┬───────┘
               │ creates
               ▼
      ┌────────────────┐
      │ IPaymentMethod │ ◄─────── Product Interface
      └────────┬───────┘
               │ implements
       ┌───────┼───────┐
       ▼       ▼       ▼
  ┌────────┐ ┌────────┐ ┌────────┐
  │  Cash  │ │Paypal  │ │ VNPay  │ ◄─── Concrete Products
  │Payment │ │Payment │ │Payment │
  └────────┘ └────────┘ └────────┘
```

---

## 🎯 KẾT LUẬN

**Factory Method Pattern** cho phép:
- ✅ Tách logic tạo objects khỏi business logic
- ✅ Giảm coupling giữa client và concrete classes
- ✅ Dễ mở rộng thêm products mới
- ✅ Follow Open/Closed Principle
- ✅ Code sạch, dễ maintain và test

**So với Singleton:**
- Singleton: **1 instance** duy nhất (Logger)
- Factory Method: **Nhiều instances** khác nhau (Payments)

**Happy Coding! 🚀**
