using System;
using Minio;
using Microsoft.Extensions.Options;
using Messenger.Application.Common.Storage;
using System.Text;
using Messenger.API.Endpoints;
using Messenger.API.Extensions;
using Messenger.Application.Auth.Services;
using Messenger.Application.Chat.Services;
using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Messenger.Application.User.Services;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Storage;
using Messenger.Persistence.DbContext;
using Messenger.Persistence.Repositories;
using Messenger.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using MinioStorageService = Messenger.Application.Common.Storage.MinioStorageService;

var builder = WebApplication.CreateBuilder(args);

// 1. Конфигурация из appsettings.json:
//    • ConnectionStrings:DefaultConnection
//    • Security:MasterKey
//    • Jwt:Secret, Jwt:Issuer, Jwt:Audience, Jwt:ExpiresHours
//    • Logging:Minio (Endpoint, AccessKey, SecretKey, BucketName, UseSSL)

// 2. EF Core + PostgreSQL
builder.Services.AddDbContext<MessengerDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 3. EncryptionService (из проекта Messenger.Security)
builder.Services.AddSingleton<EncryptionService>();

// 4. Репозитории (Messenger.Persistence)
builder.Services.AddScoped<AttachmentRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<UserRepository>();

// 5. Сервисы уровня Application
builder.Services.AddScoped<IAuthService,      AuthService>();
builder.Services.AddScoped<IChatService,      ChatService>();
builder.Services.AddScoped<IMessageService,   MessageService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IUserService,      UserService>();
builder.Services.AddScoped<IKeyStoreService,  KeyStoreService>();

// 6. MinIO-хранилище
builder.Services.Configure<MinioOptions>(
        builder.Configuration.GetSection("Minio"));

builder.Services.AddSingleton<MinioClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    return (new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .WithSSL(opts.UseSSL) as MinioClient)!;
});

// 7. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Messenger API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Bearer authorization",
        Name        = "Authorization",
        In          = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type        = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme      = JwtBearerDefaults.AuthenticationScheme
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        [ new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = JwtBearerDefaults.AuthenticationScheme
                }
            }
        ] = Array.Empty<string>()
    });
});

// 8. JWT-аутентификация
var jwtSecret = builder.Configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret missing");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            ValidateLifetime         = true
        };
    });
builder.Services.AddAuthorization();

// 9. Controllers / Endpoints
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// *attach your groups here*:
app.MapAttachmentEndpoints();
app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapAuthEndpoints();

app.Run();
