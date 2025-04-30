using Messenger.API.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Messenger.API.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Привязывает все минимальные (Minimal API) эндпоинты к маршрутизатору.
        /// </summary>
        public static void MapAllEndpoints(this IEndpointRouteBuilder app)
        {
            // Authentication
            app.MapAuthEndpoints();          // из AuthEndpoints.cs :contentReference[oaicite:0]{index=0}

            // Chat (только для авторизованных)
            app.MapChatEndpoints();          // из ChatEndpoints.cs :contentReference[oaicite:1]{index=1}

            // User (профиль, поиск, смена имени, аватар)
            app.MapUserEndpoints();          // из UserEndpoints.cs :contentReference[oaicite:2]{index=2}

            // Attachments (upload/download)
            app.MapAttachmentEndpoints();    // из AttachmentEndpoints.cs :contentReference[oaicite:3]{index=3}
        }
    }
}