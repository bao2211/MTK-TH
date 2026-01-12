using Microsoft.AspNetCore.Mvc;
using SingletonPattern.Services;

namespace SingletonPattern.Controllers
{
    /// <summary>
    /// Controller để quản lý và xem logs
    /// Minh họa việc sử dụng cùng một Singleton instance
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly LoggerService _logger;
        
        public LogController()
        {
            // Lấy instance của Singleton Logger
            _logger = LoggerService.Instance;
            _logger.LogInfo("LogController được khởi tạo", "LogController");
        }
        
        /// <summary>
        /// Lấy tất cả logs
        /// </summary>
        [HttpGet]
        public IActionResult GetAllLogs()
        {
            _logger.LogInfo("Đang lấy tất cả logs", "LogController.GetAllLogs");
            
            var logs = _logger.GetAllLogs();
            
            return Ok(new
            {
                Success = true,
                TotalLogs = logs.Count,
                Data = logs,
                LoggerInstanceId = _logger.GetInstanceId(),
                Message = "Tất cả logs từ cùng một Singleton instance"
            });
        }
        
        /// <summary>
        /// Lấy logs theo level
        /// </summary>
        [HttpGet("level/{level}")]
        public IActionResult GetLogsByLevel(string level)
        {
            _logger.LogInfo($"Đang lấy logs với level: {level}", "LogController.GetLogsByLevel");
            
            var logs = _logger.GetLogsByLevel(level);
            
            return Ok(new
            {
                Success = true,
                Level = level,
                TotalLogs = logs.Count,
                Data = logs,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Lấy thống kê logs
        /// </summary>
        [HttpGet("stats")]
        public IActionResult GetLogStats()
        {
            _logger.LogInfo("Đang lấy thống kê logs", "LogController.GetLogStats");
            
            var allLogs = _logger.GetAllLogs();
            var stats = new
            {
                TotalLogs = allLogs.Count,
                InfoLogs = allLogs.Count(l => l.Level == "INFO"),
                WarningLogs = allLogs.Count(l => l.Level == "WARNING"),
                ErrorLogs = allLogs.Count(l => l.Level == "ERROR"),
                LoggerInstanceId = _logger.GetInstanceId()
            };
            
            return Ok(new
            {
                Success = true,
                Data = stats,
                Message = "Thống kê từ Singleton Logger instance"
            });
        }
        
        /// <summary>
        /// Xóa tất cả logs
        /// </summary>
        [HttpDelete]
        public IActionResult ClearLogs()
        {
            _logger.LogWarning("Đang xóa tất cả logs", "LogController.ClearLogs");
            
            var countBeforeClear = _logger.GetLogCount();
            _logger.ClearLogs();
            
            return Ok(new
            {
                Success = true,
                Message = $"Đã xóa {countBeforeClear} logs",
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Kiểm tra Singleton pattern
        /// </summary>
        [HttpGet("verify-singleton")]
        public IActionResult VerifySingleton()
        {
            // Lấy instance nhiều lần
            var instance1 = LoggerService.Instance;
            var instance2 = LoggerService.Instance;
            var instance3 = LoggerService.Instance;
            
            var isSingleton = 
                instance1.GetInstanceId() == instance2.GetInstanceId() &&
                instance2.GetInstanceId() == instance3.GetInstanceId() &&
                instance1.GetInstanceId() == _logger.GetInstanceId();
            
            _logger.LogInfo($"Kiểm tra Singleton: {isSingleton}", "LogController.VerifySingleton");
            
            return Ok(new
            {
                Success = true,
                IsSingleton = isSingleton,
                Instance1Id = instance1.GetInstanceId(),
                Instance2Id = instance2.GetInstanceId(),
                Instance3Id = instance3.GetInstanceId(),
                CurrentInstanceId = _logger.GetInstanceId(),
                Message = isSingleton 
                    ? "✓ Tất cả đều trỏ đến cùng một instance - Singleton hoạt động đúng!" 
                    : "✗ Các instance khác nhau - Singleton không hoạt động đúng!"
            });
        }
    }
}
