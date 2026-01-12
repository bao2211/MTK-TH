using Microsoft.AspNetCore.Mvc;
using SingletonPattern.Models;
using SingletonPattern.Services;
using SingletonPattern.Services.PaymentFactory;

namespace SingletonPattern.Controllers
{
    /// <summary>
    /// Controller xử lý payments sử dụng Factory Method Pattern
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly LoggerService _logger;
        private readonly IPaymentFactory _paymentFactory;
        
        public PaymentController()
        {
            _logger = LoggerService.Instance;
            // Tạo PaymentFactory - có thể inject qua DI trong production
            _paymentFactory = new PaymentFactory();
            _logger.LogInfo("PaymentController được khởi tạo", "PaymentController");
        }
        
        /// <summary>
        /// Xử lý thanh toán - Demo Factory Method Pattern
        /// </summary>
        /// <param name="request">Thông tin thanh toán</param>
        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            _logger.LogInfo($"Nhận request thanh toán - Type: {request.PaymentType}, Amount: {request.Amount}", "PaymentController");
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(request.PaymentType))
                {
                    _logger.LogError("Payment type không được để trống", "PaymentController");
                    return BadRequest(new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment type là bắt buộc"
                    });
                }
                
                if (request.Amount <= 0)
                {
                    _logger.LogError($"Số tiền không hợp lệ: {request.Amount}", "PaymentController");
                    return BadRequest(new PaymentResponse
                    {
                        Success = false,
                        Message = "Số tiền phải lớn hơn 0"
                    });
                }
                
                // Check if payment method is supported
                if (!_paymentFactory.IsPaymentMethodSupported(request.PaymentType))
                {
                    _logger.LogWarning($"Payment method không được hỗ trợ: {request.PaymentType}", "PaymentController");
                    return BadRequest(new PaymentResponse
                    {
                        Success = false,
                        Message = $"Phương thức thanh toán '{request.PaymentType}' không được hỗ trợ",
                        Data = new
                        {
                            SupportedMethods = _paymentFactory.GetSupportedPaymentMethods()
                        }
                    });
                }
                
                // 🎯 FACTORY METHOD PATTERN - Tạo payment object dựa trên type
                IPaymentMethod paymentMethod = _paymentFactory.CreatePaymentMethod(request.PaymentType);
                
                _logger.LogInfo($"✅ Factory đã tạo {paymentMethod.PaymentType} payment method", "PaymentController");
                
                // Xử lý thanh toán
                var result = paymentMethod.ProcessPayment(request.Amount, request.OrderId, request.AdditionalData);
                
                if (result.Success)
                {
                    _logger.LogInfo($"✅ Thanh toán thành công - Transaction: {result.TransactionId}", "PaymentController");
                    return Ok(new PaymentResponse
                    {
                        Success = true,
                        Message = "Thanh toán thành công",
                        Data = result
                    });
                }
                else
                {
                    _logger.LogError($"❌ Thanh toán thất bại - {result.Message}", "PaymentController");
                    return BadRequest(new PaymentResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Data = result
                    });
                }
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError($"Payment method không hỗ trợ: {ex.Message}", "PaymentController");
                return BadRequest(new PaymentResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new
                    {
                        SupportedMethods = _paymentFactory.GetSupportedPaymentMethods()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi xử lý thanh toán: {ex.Message}", "PaymentController");
                return StatusCode(500, new PaymentResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xử lý thanh toán"
                });
            }
        }
        
        /// <summary>
        /// Lấy danh sách các phương thức thanh toán được hỗ trợ
        /// </summary>
        [HttpGet("methods")]
        public IActionResult GetPaymentMethods()
        {
            _logger.LogInfo("Lấy danh sách payment methods", "PaymentController");
            
            var methods = new List<PaymentMethodInfo>
            {
                new PaymentMethodInfo
                {
                    Type = "CASH",
                    DisplayName = "Tiền mặt",
                    Description = "Thanh toán trực tiếp bằng tiền mặt",
                    MinAmount = 0,
                    MaxAmount = 100_000_000,
                    FeeDescription = "Miễn phí",
                    RequiredFields = new List<string> { "OrderId", "Amount" }
                },
                new PaymentMethodInfo
                {
                    Type = "PAYPAL",
                    DisplayName = "PayPal",
                    Description = "Thanh toán quốc tế qua PayPal",
                    MinAmount = 0,
                    MaxAmount = decimal.MaxValue,
                    FeeDescription = "3.4% + $0.30 mỗi giao dịch",
                    RequiredFields = new List<string> { "OrderId", "Amount", "PaypalEmail" }
                },
                new PaymentMethodInfo
                {
                    Type = "VNPAY",
                    DisplayName = "VNPay",
                    Description = "Thanh toán qua cổng VNPay",
                    MinAmount = 10_000,
                    MaxAmount = decimal.MaxValue,
                    FeeDescription = "2% (tối đa 50,000 VNĐ)",
                    RequiredFields = new List<string> { "OrderId", "Amount", "BankCode" }
                }
            };
            
            return Ok(new PaymentResponse
            {
                Success = true,
                Message = "Danh sách phương thức thanh toán",
                Data = methods
            });
        }
        
        /// <summary>
        /// Tính phí giao dịch cho payment method
        /// </summary>
        [HttpGet("calculate-fee")]
        public IActionResult CalculateFee([FromQuery] string paymentType, [FromQuery] decimal amount)
        {
            _logger.LogInfo($"Tính phí cho {paymentType} với amount: {amount}", "PaymentController");
            
            try
            {
                if (!_paymentFactory.IsPaymentMethodSupported(paymentType))
                {
                    return BadRequest(new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment method không được hỗ trợ"
                    });
                }
                
                // Factory tạo payment method để tính phí
                var paymentMethod = _paymentFactory.CreatePaymentMethod(paymentType);
                var fee = paymentMethod.GetTransactionFee(amount);
                var totalAmount = amount + fee;
                
                return Ok(new PaymentResponse
                {
                    Success = true,
                    Message = "Tính phí thành công",
                    Data = new
                    {
                        PaymentType = paymentMethod.PaymentType,
                        Amount = amount,
                        TransactionFee = fee,
                        TotalAmount = totalAmount,
                        FeePercentage = amount > 0 ? (fee / amount * 100) : 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính phí: {ex.Message}", "PaymentController");
                return BadRequest(new PaymentResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        
        /// <summary>
        /// Demo Factory Pattern - Tạo nhiều payment methods
        /// </summary>
        [HttpGet("demo-factory")]
        public IActionResult DemoFactory()
        {
            _logger.LogInfo("Demo Factory Method Pattern", "PaymentController");
            
            var results = new List<object>();
            var paymentTypes = new[] { "CASH", "PAYPAL", "VNPAY" };
            
            foreach (var type in paymentTypes)
            {
                // Factory tạo từng payment method
                var payment = _paymentFactory.CreatePaymentMethod(type);
                
                results.Add(new
                {
                    PaymentType = payment.PaymentType,
                    ClassName = payment.GetType().Name,
                    HashCode = payment.GetHashCode(),
                    SampleFee = payment.GetTransactionFee(1000000)
                });
            }
            
            return Ok(new PaymentResponse
            {
                Success = true,
                Message = "Factory Method Pattern Demo - Mỗi lần gọi factory tạo instance mới",
                Data = new
                {
                    PaymentMethods = results,
                    FactoryType = _paymentFactory.GetType().Name,
                    Note = "Factory Method cho phép tạo các objects khác nhau mà không cần biết class cụ thể"
                }
            });
        }
    }
}
