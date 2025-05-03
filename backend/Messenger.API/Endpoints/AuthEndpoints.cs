using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Messenger.Application.Auth.DTOs.LoginRequest;
using RegisterRequest = Messenger.Application.Auth.DTOs.RegisterRequest;

namespace Messenger.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .RequireCors("AllowLocalDev") // Для поддержки CORS
            .WithTags("Authentication");

        // Регистрация
        group.MapPost("/register", async (
                [FromBody] RegisterRequest request,
                IAuthService authService) =>
            {
                var result = await authService.RegisterAsync(request);
                return Results.Ok(result);
            })
            .WithName("Register");

        // Вход (логин)
        group.MapPost("/login", async (
                [FromBody] LoginRequest request,
                IAuthService authService) =>
            {
                var token = await authService.LoginAsync(request);
                return token is not null
                    ? Results.Ok(new { token })
                    : Results.Unauthorized();
            })
            .WithName("Login");
    }
}