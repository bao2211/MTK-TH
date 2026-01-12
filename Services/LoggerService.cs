namespace SingletonPattern.Services
{
    /// <summary>
    /// Logger Service implementing Singleton Pattern
    /// Đảm bảo chỉ có một instance duy nhất trong toàn bộ ứng dụng
    /// </summary>
    public sealed class LoggerService
    {
        // Instance duy nhất của Logger
        private static LoggerService? _instance;
        
        // Lock object để đảm bảo thread-safety
        private static readonly object _lock = new object();
        
        // Danh sách log messages
        private readonly List<LogEntry> _logs;
        
        // Private constructor để ngăn việc tạo instance từ bên ngoài
        private LoggerService()
        {
            _logs = new List<LogEntry>();
            Console.WriteLine($"[SINGLETON] LoggerService instance được khởi tạo lúc {DateTime.Now:HH:mm:ss.fff}");
        }
        
        /// <summary>
        /// Thuộc tính để lấy instance duy nhất (Thread-safe với Double-Check Locking)
        /// </summary>
        public static LoggerService Instance
        {
            get
            {
                // Double-check locking pattern
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggerService();
                        }
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Ghi log với level INFO
        /// </summary>
        public void LogInfo(string message, string source)
        {
            Log("INFO", message, source);
        }
        
        /// <summary>
        /// Ghi log với level WARNING
        /// </summary>
        public void LogWarning(string message, string source)
        {
            Log("WARNING", message, source);
        }
        
        /// <summary>
        /// Ghi log với level ERROR
        /// </summary>
        public void LogError(string message, string source)
        {
            Log("ERROR", message, source);
        }
        
        /// <summary>
        /// Phương thức private để ghi log
        /// </summary>
        private void Log(string level, string message, string source)
        {
            lock (_lock)
            {
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Message = message,
                    Source = source
                };
                
                _logs.Add(logEntry);
                
                // In ra console với màu sắc phù hợp
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = level switch
                {
                    "ERROR" => ConsoleColor.Red,
                    "WARNING" => ConsoleColor.Yellow,
                    "INFO" => ConsoleColor.Green,
                    _ => ConsoleColor.White
                };
                
                Console.WriteLine($"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] [{source}] {message}");
                Console.ForegroundColor = originalColor;
            }
        }
        
        /// <summary>
        /// Lấy tất cả logs
        /// </summary>
        public List<LogEntry> GetAllLogs()
        {
            lock (_lock)
            {
                return new List<LogEntry>(_logs);
            }
        }
        
        /// <summary>
        /// Lấy logs theo level
        /// </summary>
        public List<LogEntry> GetLogsByLevel(string level)
        {
            lock (_lock)
            {
                return _logs.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
        
        /// <summary>
        /// Xóa tất cả logs
        /// </summary>
        public void ClearLogs()
        {
            lock (_lock)
            {
                _logs.Clear();
                Console.WriteLine("[SINGLETON] Logs đã được xóa");
            }
        }
        
        /// <summary>
        /// Lấy tổng số logs
        /// </summary>
        public int GetLogCount()
        {
            lock (_lock)
            {
                return _logs.Count;
            }
        }
        
        /// <summary>
        /// Lấy ID của instance để kiểm tra Singleton
        /// </summary>
        public string GetInstanceId()
        {
            return GetHashCode().ToString();
        }
    }
    
    /// <summary>
    /// Class đại diện cho một log entry
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}
