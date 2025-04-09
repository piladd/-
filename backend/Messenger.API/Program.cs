using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Data;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 📦 Регистрация контекста БД
builder.Services.AddDbContext<MessengerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 📦 Репозитории и сервисы
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IKeyStoreService, KeyStoreService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // ✅ добавили AuthService

// 🔐 JWT конфигурация
var jwtKey = builder.Configuration["JWT_KEY"];
var jwtIssuer = builder.Configuration["JWT_ISSUER"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IKeyStoreService, KeyStoreService>();

var app = builder.Build();

// 🔧 Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication(); // ✅ обязательно ДО UseAuthorization
app.UseAuthorization();
app.MapControllers();

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
    db.Database.Migrate();
}

app.Run();