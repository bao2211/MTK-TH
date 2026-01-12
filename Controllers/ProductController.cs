using Microsoft.AspNetCore.Mvc;
using SingletonPattern.Services;

namespace SingletonPattern.Controllers
{
    /// <summary>
    /// Controller cho Product Management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly LoggerService _logger;
        
        public ProductController()
        {
            // Lấy instance của Singleton Logger
            _logger = LoggerService.Instance;
            _logger.LogInfo("ProductController được khởi tạo", "ProductController");
        }
        
        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        [HttpGet]
        public IActionResult GetProducts()
        {
            _logger.LogInfo("Đang lấy danh sách sản phẩm", "ProductController.GetProducts");
            
            var products = new[]
            {
                new { Id = 1, Name = "Laptop Dell XPS 15", Price = 35000000 },
                new { Id = 2, Name = "iPhone 15 Pro Max", Price = 30000000 },
                new { Id = 3, Name = "Samsung Galaxy S24", Price = 25000000 }
            };
            
            _logger.LogInfo($"Đã lấy {products.Length} sản phẩm thành công", "ProductController.GetProducts");
            
            return Ok(new
            {
                Success = true,
                Data = products,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string keyword)
        {
            _logger.LogInfo($"Tìm kiếm sản phẩm với từ khóa: {keyword}", "ProductController.SearchProducts");
            
            if (string.IsNullOrEmpty(keyword))
            {
                _logger.LogWarning("Từ khóa tìm kiếm rỗng", "ProductController.SearchProducts");
                return BadRequest(new { Success = false, Message = "Từ khóa không được để trống" });
            }
            
            var results = new[]
            {
                new { Id = 1, Name = $"Kết quả cho '{keyword}'", Price = 10000000 }
            };
            
            _logger.LogInfo($"Tìm thấy {results.Length} sản phẩm", "ProductController.SearchProducts");
            
            return Ok(new
            {
                Success = true,
                Data = results,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _logger.LogWarning($"Đang xóa sản phẩm với ID: {id}", "ProductController.DeleteProduct");
            
            if (id <= 0)
            {
                _logger.LogError($"ID không hợp lệ khi xóa: {id}", "ProductController.DeleteProduct");
                return BadRequest(new { Success = false, Message = "ID không hợp lệ" });
            }
            
            _logger.LogInfo($"Sản phẩm ID {id} đã được xóa", "ProductController.DeleteProduct");
            
            return Ok(new
            {
                Success = true,
                Message = $"Đã xóa sản phẩm ID {id}",
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
    }
}
