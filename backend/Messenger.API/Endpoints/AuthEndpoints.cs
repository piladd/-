using Messenger.Application.Interfaces;
using LoginRequest = Messenger.Application.Auth.DTOs.LoginRequest;
using RegisterRequest = Messenger.Application.Auth.DTOs.RegisterRequest;
using AuthResponse = Messenger.Application.Auth.DTOs.AuthResponse;

namespace Messenger.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // Регистрация: принимает RegisterRequest с PublicKey + PrivateKey, возвращает AuthResponse
        app.MapPost("/api/auth/register",
                async (RegisterRequest request, IAuthService authService) =>
                {
                    var response = await authService.RegisterAsync(request);
                    return Results.Ok(response);
                })
            .WithName("Register")
            .Accepts<RegisterRequest>("application/json")
            .Produces<AuthResponse>(StatusCodes.Status200OK);

        // Логин: принимает LoginRequest, возвращает AuthResponse (PublicKey + token)
        app.MapPost("/api/auth/login",
                async (LoginRequest request, IAuthService authService) =>
                {
                    // Передаём весь объект LoginRequest
                    var response = await authService.LoginAsync(request);
                    return response is null
                        ? Results.Unauthorized()
                        : Results.Ok(response);
                })
            .WithName("Login")
            .Accepts<LoginRequest>("application/json")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}