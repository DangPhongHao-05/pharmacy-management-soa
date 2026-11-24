using Core_Service.Data;
using Core_Service.Interfaces.Repositories;
using Core_Service.Interfaces.Services;
using Core_Service.Repositories;
using Core_Service.Services;
using Microsoft.EntityFrameworkCore;
using Core_Service.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký kết nối PostgreSQL (Supabase)
var connectionString = builder.Configuration.GetConnectionString("Supabase");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. Đăng ký Repositories & Services
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IImportService, ImportService>();

var app = builder.Build();
// Đăng ký Middleware bắt lỗi toàn cục
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();