using System;
using System.Threading.Tasks;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Messenger.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnUserDto_WhenCredentialsAreCorrect()
    {
        // Arrange
        var mockUserRepo = new Mock<UserRepository>(null);
        var encryptionService = new EncryptionService();
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Crypto:MasterKey"]).Returns("TestSecretKey1234567890123456");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = encryptionService.EncryptWithPublicKey("password", encryptionService.GenerateAsymmetricKeys().publicKey) // just placeholder
        };

        mockUserRepo.Setup(repo => repo.GetUserByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        var authService = new AuthService(mockUserRepo.Object, encryptionService, mockConfig.Object);

        // Act
        var result = await authService.AuthenticateAsync("testuser", "password");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }
}

[Fact]
public async Task ShouldReturnNull_WhenUserDoesNotExist()
{
    // Arrange
    var mockUserRepo = new Mock<UserRepository>(null);
    var encryptionService = new EncryptionService();
    var mockConfig = new Mock<IConfiguration>();
    mockConfig.Setup(c => c["Crypto:MasterKey"]).Returns("TestSecretKey1234567890123456");

    mockUserRepo.Setup(repo => repo.GetUserByUsernameAsync("wronguser"))
        .ReturnsAsync((User)null);

    var authService = new AuthService(mockUserRepo.Object, encryptionService, mockConfig.Object);

    // Act
    var result = await authService.AuthenticateAsync("wronguser", "password");

    // Assert
    Assert.Null(result);
}

}