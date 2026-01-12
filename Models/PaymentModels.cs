namespace SingletonPattern.Models
{
    /// <summary>
    /// Request model cho payment processing
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Loại thanh toán: CASH, PAYPAL, VNPAY
        /// </summary>
        public string PaymentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string OrderId { get; set; } = string.Empty;
        
        /// <summary>
        /// Thông tin bổ sung (tùy theo payment type)
        /// - CASH: ReceivedAmount, ChangeAmount, Cashier
        /// - PAYPAL: PaypalEmail
        /// - VNPAY: BankCode, CardNumber
        /// </summary>
        public Dictionary<string, string>? AdditionalData { get; set; }
    }
    
    /// <summary>
    /// Response model cho payment
    /// </summary>
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
    
    /// <summary>
    /// Model cho danh sách payment methods
    /// </summary>
    public class PaymentMethodInfo
    {
        public string Type { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string FeeDescription { get; set; } = string.Empty;
        public List<string> RequiredFields { get; set; } = new();
    }
}
