using Minio;
using Microsoft.OpenApi.Models;
using Messenger.Application.Common.Storage;
using Messenger.Infrastructure.Storage;
using Messenger.Application.Auth.Services;
using Messenger.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection("Minio"));

        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = configuration.GetSection("Minio").Get<MinioOptions>();

            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .Build();
        });

        services.AddScoped<IStorageService, MinioStorageService>();

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Savrani Messenger API",
                Version = "v1"
            });

            var jwtScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Введите JWT токен. Пример: **Bearer {токен}**"
            };

            opt.AddSecurityDefinition("Bearer", jwtScheme);
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtScheme, Array.Empty<string>() }
            });
        });

        return services;
    }
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Ядро (core-сервис регистрации с ключами)
        services.AddScoped<AuthService>();

        // API-интерфейс — работа с JWT и возврат токена
        services.AddScoped<IAuthService, ApiAuthService>();

        // TODO: здесь потом добавим валидаторы и другие модули (UserService, ChatService и т.п.)

        return services;
    }
}