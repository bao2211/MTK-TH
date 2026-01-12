namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Concrete Factory - Implementation của Factory Method Pattern
    /// Chịu trách nhiệm tạo các concrete payment objects
    /// </summary>
    public class PaymentFactory : IPaymentFactory
    {
        private readonly LoggerService _logger;
        
        // Dictionary để track số lượng payment methods đã được tạo
        private readonly Dictionary<string, int> _creationStats;
        
        public PaymentFactory()
        {
            _logger = LoggerService.Instance;
            _creationStats = new Dictionary<string, int>();
            _logger.LogInfo("PaymentFactory được khởi tạo", "PaymentFactory");
        }
        
        /// <summary>
        /// Factory Method - Core của pattern
        /// Quyết định class nào sẽ được instantiate dựa trên paymentType
        /// </summary>
        public IPaymentMethod CreatePaymentMethod(string paymentType)
        {
            _logger.LogInfo($"Factory đang tạo payment method: {paymentType}", "PaymentFactory");
            
            // Normalize input
            var normalizedType = paymentType.ToUpper().Trim();
            
            // Factory Method logic - Quyết định tạo object nào
            IPaymentMethod paymentMethod = normalizedType switch
            {
                "CASH" or "TIỀN MẶT" or "TIEN_MAT" => new CashPayment(),
                "PAYPAL" => new PaypalPayment(),
                "VNPAY" or "VN_PAY" => new VNPayPayment(),
                _ => throw new NotSupportedException($"Phương thức thanh toán '{paymentType}' không được hỗ trợ")
            };
            
            // Track statistics
            if (!_creationStats.ContainsKey(normalizedType))
            {
                _creationStats[normalizedType] = 0;
            }
            _creationStats[normalizedType]++;
            
            _logger.LogInfo($"✅ Factory đã tạo {paymentMethod.PaymentType} payment method (Total created: {_creationStats[normalizedType]})", "PaymentFactory");
            
            return paymentMethod;
        }
        
        /// <summary>
        /// Lấy danh sách payment methods được hỗ trợ
        /// </summary>
        public List<string> GetSupportedPaymentMethods()
        {
            return new List<string> { "CASH", "PAYPAL", "VNPAY" };
        }
        
        /// <summary>
        /// Kiểm tra payment method có được hỗ trợ không
        /// </summary>
        public bool IsPaymentMethodSupported(string paymentType)
        {
            var normalizedType = paymentType.ToUpper().Trim();
            var supportedTypes = new[] { "CASH", "TIỀN MẶT", "TIEN_MAT", "PAYPAL", "VNPAY", "VN_PAY" };
            return supportedTypes.Contains(normalizedType);
        }
        
        /// <summary>
        /// Lấy thống kê số lượng payment methods đã được tạo
        /// </summary>
        public Dictionary<string, int> GetCreationStatistics()
        {
            return new Dictionary<string, int>(_creationStats);
        }
        
        /// <summary>
        /// Reset statistics (for testing purposes)
        /// </summary>
        public void ResetStatistics()
        {
            _creationStats.Clear();
            _logger.LogInfo("Factory statistics đã được reset", "PaymentFactory");
        }
    }
}
