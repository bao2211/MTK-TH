namespace SingletonPattern.Services.PaymentFactory
{
    /// <summary>
    /// Factory Interface - Định nghĩa contract cho việc tạo Payment objects
    /// Đây là Creator trong Factory Method Pattern
    /// </summary>
    public interface IPaymentFactory
    {
        /// <summary>
        /// Factory Method - Tạo payment method dựa trên loại thanh toán
        /// </summary>
        /// <param name="paymentType">Loại thanh toán: CASH, PAYPAL, VNPAY</param>
        /// <returns>Instance của IPaymentMethod tương ứng</returns>
        IPaymentMethod CreatePaymentMethod(string paymentType);
        
        /// <summary>
        /// Lấy danh sách các phương thức thanh toán được hỗ trợ
        /// </summary>
        List<string> GetSupportedPaymentMethods();
        
        /// <summary>
        /// Kiểm tra xem phương thức thanh toán có được hỗ trợ không
        /// </summary>
        bool IsPaymentMethodSupported(string paymentType);
    }
}
