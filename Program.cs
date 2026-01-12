using SingletonPattern.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Singleton Pattern Demo - Logger Service API",
        Version = "v1",
        Description = "Lab thực hành Singleton Pattern với Logger Service trong ASP.NET Core Web API"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Singleton Pattern API v1");
        c.RoutePrefix = string.Empty; // Mở Swagger UI tại root URL
    });
}

// Log khởi động ứng dụng
var logger = LoggerService.Instance;
logger.LogInfo("===== ỨNG DỤNG KHỞI ĐỘNG =====", "Program");
logger.LogInfo($"Logger Instance ID: {logger.GetInstanceId()}", "Program");
logger.LogInfo("Singleton Pattern đã được khởi tạo", "Program");

app.UseAuthorization();

app.MapControllers();

// Endpoint kiểm tra health
app.MapGet("/health", () =>
{
    logger.LogInfo("Health check được gọi", "HealthCheck");
    return Results.Ok(new
    {
        Status = "Healthy",
        Timestamp = DateTime.Now,
        LoggerInstanceId = logger.GetInstanceId()
    });
});

logger.LogInfo($"API đang chạy trên: {app.Urls.FirstOrDefault() ?? "http://localhost:5000"}", "Program");

app.Run();
