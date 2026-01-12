namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Interface định nghĩa contract cho các phương thức thanh toán
    /// Đây là Product trong Factory Method Pattern
    /// </summary>
    public interface IPaymentMethod
    {
        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        string PaymentType { get; }
        
        /// <summary>
        /// Xử lý thanh toán
        /// </summary>
        /// <param name="amount">Số tiền cần thanh toán</param>
        /// <param name="orderId">Mã đơn hàng</param>
        /// <param name="additionalData">Dữ liệu bổ sung (tùy từng phương thức)</param>
        /// <returns>Kết quả thanh toán</returns>
        PaymentResult ProcessPayment(decimal amount, string orderId, Dictionary<string, string>? additionalData = null);
        
        /// <summary>
        /// Kiểm tra tính hợp lệ của thông tin thanh toán
        /// </summary>
        bool ValidatePayment(decimal amount, Dictionary<string, string>? additionalData = null);
        
        /// <summary>
        /// Lấy phí giao dịch
        /// </summary>
        decimal GetTransactionFee(decimal amount);
    }
    
    /// <summary>
    /// Class chứa kết quả thanh toán
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public Dictionary<string, string>? AdditionalInfo { get; set; }
    }
}
