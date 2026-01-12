namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Concrete Product - Thanh toán qua Paypal
    /// </summary>
    public class PaypalPayment : IPaymentMethod
    {
        private readonly LoggerService _logger;
        
        public string PaymentType => "PAYPAL";
        
        public PaypalPayment()
        {
            _logger = LoggerService.Instance;
            _logger.LogInfo("PaypalPayment instance được tạo", "PaypalPayment");
        }
        
        public PaymentResult ProcessPayment(decimal amount, string orderId, Dictionary<string, string>? additionalData = null)
        {
            _logger.LogInfo($"Xử lý thanh toán Paypal - Order: {orderId}, Amount: ${amount}", "PaypalPayment");
            
            if (!ValidatePayment(amount, additionalData))
            {
                _logger.LogError($"Thanh toán Paypal thất bại - Validation failed", "PaypalPayment");
                return new PaymentResult
                {
                    Success = false,
                    Message = "Thông tin Paypal không hợp lệ",
                    PaymentType = PaymentType,
                    ProcessedAt = DateTime.Now
                };
            }
            
            var paypalEmail = additionalData?.GetValueOrDefault("PaypalEmail", "");
            
            // Simulate Paypal API call
            _logger.LogInfo($"Đang gọi Paypal API cho email: {paypalEmail}", "PaypalPayment");
            System.Threading.Thread.Sleep(1500); // Giả lập API call
            
            var transactionFee = GetTransactionFee(amount);
            var totalAmount = amount + transactionFee;
            var transactionId = $"PAYPAL-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            
            _logger.LogInfo($"Thanh toán Paypal thành công - Transaction ID: {transactionId}", "PaypalPayment");
            
            return new PaymentResult
            {
                Success = true,
                Message = "Thanh toán Paypal thành công",
                TransactionId = transactionId,
                PaymentType = PaymentType,
                Amount = amount,
                TransactionFee = transactionFee,
                TotalAmount = totalAmount,
                ProcessedAt = DateTime.Now,
                AdditionalInfo = new Dictionary<string, string>
                {
                    { "PaypalEmail", paypalEmail ?? "N/A" },
                    { "PaypalTransactionId", $"PP-{new Random().Next(100000, 999999)}" },
                    { "Currency", "USD" },
                    { "ExchangeRate", "23500" }
                }
            };
        }
        
        public bool ValidatePayment(decimal amount, Dictionary<string, string>? additionalData = null)
        {
            // Kiểm tra số tiền
            if (amount <= 0)
            {
                _logger.LogWarning($"Số tiền không hợp lệ: {amount}", "PaypalPayment");
                return false;
            }
            
            // Kiểm tra Paypal email
            if (additionalData == null || !additionalData.ContainsKey("PaypalEmail"))
            {
                _logger.LogWarning("Thiếu thông tin Paypal Email", "PaypalPayment");
                return false;
            }
            
            var email = additionalData["PaypalEmail"];
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                _logger.LogWarning($"Email Paypal không hợp lệ: {email}", "PaypalPayment");
                return false;
            }
            
            return true;
        }
        
        public decimal GetTransactionFee(decimal amount)
        {
            // Paypal: 3.4% + $0.30 per transaction
            var feePercent = amount * 0.034m;
            var fixedFee = 0.30m;
            return feePercent + fixedFee;
        }
    }
}
