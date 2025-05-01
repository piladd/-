using Messenger.Application.Attachment.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Messenger.Application.Interfaces;
using Messenger.Application.User.Services;
using Messenger.Application.Auth.Services;
using Messenger.Application.Chat.Services;
using Messenger.Application.Attachment.Services;
using Messenger.Application.Common.Storage;
using Messenger.Persistence.Repositories;

namespace Messenger.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core application services
        services.AddScoped<IKeyStoreService, KeyStoreService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IAttachmentService, AttachmentService>();

        // Infrastructure / storage / repositories
        services.AddScoped<IStorageService, MinioStorageService>();
        services.AddScoped<AttachmentRepository>();
        services.AddScoped<UserRepository>();
        services.AddScoped<ChatRepository>();
        services.AddScoped<MessageRepository>();

        return services;
    }
}