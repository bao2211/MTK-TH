namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Concrete Product - Thanh toán qua VNPay
    /// </summary>
    public class VNPayPayment : IPaymentMethod
    {
        private readonly LoggerService _logger;
        
        public string PaymentType => "VNPAY";
        
        public VNPayPayment()
        {
            _logger = LoggerService.Instance;
            _logger.LogInfo("VNPayPayment instance được tạo", "VNPayPayment");
        }
        
        public PaymentResult ProcessPayment(decimal amount, string orderId, Dictionary<string, string>? additionalData = null)
        {
            _logger.LogInfo($"Xử lý thanh toán VNPay - Order: {orderId}, Amount: {amount:N0} VNĐ", "VNPayPayment");
            
            if (!ValidatePayment(amount, additionalData))
            {
                _logger.LogError($"Thanh toán VNPay thất bại - Validation failed", "VNPayPayment");
                return new PaymentResult
                {
                    Success = false,
                    Message = "Thông tin VNPay không hợp lệ",
                    PaymentType = PaymentType,
                    ProcessedAt = DateTime.Now
                };
            }
            
            var bankCode = additionalData?.GetValueOrDefault("BankCode", "");
            var cardNumber = additionalData?.GetValueOrDefault("CardNumber", "");
            
            // Simulate VNPay API call
            _logger.LogInfo($"Đang kết nối VNPay Gateway - Bank: {bankCode}", "VNPayPayment");
            System.Threading.Thread.Sleep(1200); // Giả lập API call
            
            var transactionFee = GetTransactionFee(amount);
            var totalAmount = amount + transactionFee;
            var transactionId = $"VNPAY-{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
            
            _logger.LogInfo($"Thanh toán VNPay thành công - Transaction ID: {transactionId}", "VNPayPayment");
            
            return new PaymentResult
            {
                Success = true,
                Message = "Thanh toán VNPay thành công",
                TransactionId = transactionId,
                PaymentType = PaymentType,
                Amount = amount,
                TransactionFee = transactionFee,
                TotalAmount = totalAmount,
                ProcessedAt = DateTime.Now,
                AdditionalInfo = new Dictionary<string, string>
                {
                    { "BankCode", bankCode ?? "N/A" },
                    { "CardNumber", cardNumber != null ? MaskCardNumber(cardNumber) : "N/A" },
                    { "VNPayTransactionId", $"VNP{new Random().Next(10000000, 99999999)}" },
                    { "Currency", "VND" },
                    { "Gateway", "VNPay Payment Gateway" }
                }
            };
        }
        
        public bool ValidatePayment(decimal amount, Dictionary<string, string>? additionalData = null)
        {
            // Kiểm tra số tiền
            if (amount <= 0)
            {
                _logger.LogWarning($"Số tiền không hợp lệ: {amount}", "VNPayPayment");
                return false;
            }
            
            // VNPay yêu cầu số tiền tối thiểu 10,000 VNĐ
            if (amount < 10_000)
            {
                _logger.LogWarning($"Số tiền dưới mức tối thiểu: {amount:N0} VNĐ (min: 10,000 VNĐ)", "VNPayPayment");
                return false;
            }
            
            // Kiểm tra Bank Code
            if (additionalData == null || !additionalData.ContainsKey("BankCode"))
            {
                _logger.LogWarning("Thiếu thông tin mã ngân hàng (BankCode)", "VNPayPayment");
                return false;
            }
            
            var bankCode = additionalData["BankCode"];
            var validBankCodes = new[] { "NCB", "VIETCOMBANK", "VIETINBANK", "TECHCOMBANK", "MBBANK", "ACB", "BIDV", "AGRIBANK", "SACOMBANK" };
            
            if (string.IsNullOrEmpty(bankCode) || !validBankCodes.Contains(bankCode.ToUpper()))
            {
                _logger.LogWarning($"Mã ngân hàng không hợp lệ: {bankCode}", "VNPayPayment");
                return false;
            }
            
            return true;
        }
        
        public decimal GetTransactionFee(decimal amount)
        {
            // VNPay: 2% transaction fee, max 50,000 VNĐ
            var fee = amount * 0.02m;
            return Math.Min(fee, 50_000);
        }
        
        /// <summary>
        /// Ẩn một phần số thẻ để bảo mật
        /// </summary>
        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 8)
                return "****";
            
            return $"{cardNumber.Substring(0, 4)} **** **** {cardNumber.Substring(cardNumber.Length - 4)}";
        }
    }
}
