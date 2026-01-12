using Microsoft.AspNetCore.Mvc;
using SingletonPattern.Services;

namespace SingletonPattern.Controllers
{
    /// <summary>
    /// Controller cho User Management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly LoggerService _logger;
        
        public UserController()
        {
            // Lấy instance của Singleton Logger
            _logger = LoggerService.Instance;
            _logger.LogInfo("UserController được khởi tạo", "UserController");
        }
        
        /// <summary>
        /// Lấy danh sách users
        /// </summary>
        [HttpGet]
        public IActionResult GetUsers()
        {
            _logger.LogInfo("Đang lấy danh sách users", "UserController.GetUsers");
            
            var users = new[]
            {
                new { Id = 1, Name = "Nguyễn Văn A", Email = "nva@example.com" },
                new { Id = 2, Name = "Trần Thị B", Email = "ttb@example.com" },
                new { Id = 3, Name = "Lê Văn C", Email = "lvc@example.com" }
            };
            
            _logger.LogInfo($"Đã lấy {users.Length} users thành công", "UserController.GetUsers");
            
            return Ok(new
            {
                Success = true,
                Data = users,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Lấy user theo ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            _logger.LogInfo($"Đang tìm user với ID: {id}", "UserController.GetUserById");
            
            if (id <= 0)
            {
                _logger.LogWarning($"ID không hợp lệ: {id}", "UserController.GetUserById");
                return BadRequest(new { Success = false, Message = "ID phải lớn hơn 0" });
            }
            
            if (id > 10)
            {
                _logger.LogError($"Không tìm thấy user với ID: {id}", "UserController.GetUserById");
                return NotFound(new { Success = false, Message = "User không tồn tại" });
            }
            
            var user = new { Id = id, Name = $"User {id}", Email = $"user{id}@example.com" };
            _logger.LogInfo($"Tìm thấy user: {user.Name}", "UserController.GetUserById");
            
            return Ok(new
            {
                Success = true,
                Data = user,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
        
        /// <summary>
        /// Tạo user mới
        /// </summary>
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            _logger.LogInfo($"Đang tạo user mới: {request.Name}", "UserController.CreateUser");
            
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogError("Tên user không được để trống", "UserController.CreateUser");
                return BadRequest(new { Success = false, Message = "Tên không được để trống" });
            }
            
            var newUser = new
            {
                Id = new Random().Next(100, 999),
                Name = request.Name,
                Email = request.Email
            };
            
            _logger.LogInfo($"User mới được tạo thành công với ID: {newUser.Id}", "UserController.CreateUser");
            
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, new
            {
                Success = true,
                Data = newUser,
                LoggerInstanceId = _logger.GetInstanceId()
            });
        }
    }
    
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
