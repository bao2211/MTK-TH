namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Concrete Product - Thanh toán tiền mặt
    /// </summary>
    public class CashPayment : IPaymentMethod
    {
        private readonly LoggerService _logger;
        
        public string PaymentType => "CASH";
        
        public CashPayment()
        {
            _logger = LoggerService.Instance;
            _logger.LogInfo("CashPayment instance được tạo", "CashPayment");
        }
        
        public PaymentResult ProcessPayment(decimal amount, string orderId, Dictionary<string, string>? additionalData = null)
        {
            _logger.LogInfo($"Xử lý thanh toán tiền mặt - Order: {orderId}, Amount: {amount:N0} VNĐ", "CashPayment");
            
            if (!ValidatePayment(amount, additionalData))
            {
                _logger.LogError($"Thanh toán tiền mặt thất bại - Validation failed", "CashPayment");
                return new PaymentResult
                {
                    Success = false,
                    Message = "Số tiền không hợp lệ",
                    PaymentType = PaymentType,
                    ProcessedAt = DateTime.Now
                };
            }
            
            // Simulate payment processing
            System.Threading.Thread.Sleep(500); // Giả lập thời gian xử lý
            
            var transactionFee = GetTransactionFee(amount);
            var totalAmount = amount + transactionFee;
            var transactionId = $"CASH-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
            
            _logger.LogInfo($"Thanh toán tiền mặt thành công - Transaction ID: {transactionId}", "CashPayment");
            
            return new PaymentResult
            {
                Success = true,
                Message = "Thanh toán tiền mặt thành công",
                TransactionId = transactionId,
                PaymentType = PaymentType,
                Amount = amount,
                TransactionFee = transactionFee,
                TotalAmount = totalAmount,
                ProcessedAt = DateTime.Now,
                AdditionalInfo = new Dictionary<string, string>
                {
                    { "ReceivedAmount", additionalData?.GetValueOrDefault("ReceivedAmount", "0") ?? "0" },
                    { "ChangeAmount", additionalData?.GetValueOrDefault("ChangeAmount", "0") ?? "0" },
                    { "Cashier", additionalData?.GetValueOrDefault("Cashier", "Unknown") ?? "Unknown" }
                }
            };
        }
        
        public bool ValidatePayment(decimal amount, Dictionary<string, string>? additionalData = null)
        {
            // Tiền mặt: Kiểm tra số tiền > 0 và không quá lớn
            if (amount <= 0)
            {
                _logger.LogWarning($"Số tiền không hợp lệ: {amount}", "CashPayment");
                return false;
            }
            
            if (amount > 100_000_000) // Giới hạn 100 triệu
            {
                _logger.LogWarning($"Số tiền vượt quá giới hạn: {amount:N0} VNĐ", "CashPayment");
                return false;
            }
            
            return true;
        }
        
        public decimal GetTransactionFee(decimal amount)
        {
            // Tiền mặt không có phí giao dịch
            return 0;
        }
    }
}
