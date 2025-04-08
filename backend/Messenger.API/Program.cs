using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация DbContext с использованием строки подключения
builder.Services.AddDbContext<MessengerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозиториев и сервисов
builder.Services.AddScoped<ChatRepository>();  // Регистрация ChatRepository
builder.Services.AddScoped<IChatService, ChatService>();  // Регистрация IChatService и его реализации
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройки Swagger и API
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Применение миграций при старте приложения
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
    db.Database.Migrate();  // Применение миграций
}

app.Run();