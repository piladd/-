using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Messenger.Application.Auth.DTOs.LoginRequest;
using RegisterRequest = Messenger.Application.Auth.DTOs.RegisterRequest;
using AuthResponse = Messenger.Application.Auth.DTOs.AuthResponse;

namespace Messenger.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .RequireCors("AllowLocalDev")
            .WithTags("Authentication");

        // Регистрация
        group.MapPost("/register", async (
                [FromBody] RegisterRequest request,
                IAuthService authService) =>
            {
                AuthResponse result = await authService.RegisterAsync(request);
                return Results.Ok(result);
            })
            .WithName("Register");

        // Вход (логин)
        group.MapPost("/login", async (
                [FromBody] LoginRequest request,
                IAuthService authService) =>
            {
                AuthResponse? result = await authService.LoginAsync(request);
                return result is not null
                    ? Results.Ok(result)
                    : Results.Unauthorized();
            })
            .WithName("Login");
    }
}