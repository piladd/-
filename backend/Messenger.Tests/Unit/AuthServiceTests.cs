using System;
using System.Threading.Tasks;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Messenger.Tests.TestData;

namespace Messenger.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
    {
        // Arrange
        var mockUserRepo = new Mock<UserRepository>(null);
        var encryptionService = new EncryptionService();
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Security:MasterKey"]).Returns("TestSecretKey1234567890123456");

        var user = UserTestData.GetSampleUser();

        mockUserRepo.Setup(repo => repo.GetUserByUsernameAsync(user.Username))
            .ReturnsAsync(user);

        var authService = new AuthService(mockUserRepo.Object, encryptionService, mockConfig.Object);

        // Act
        var result = await authService.AuthenticateAsync(user.Username, user.Password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.PublicKey, result.PublicKey);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var mockUserRepo = new Mock<UserRepository>(null);
        var encryptionService = new EncryptionService();
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Security:MasterKey"]).Returns("TestSecretKey1234567890123456");

        mockUserRepo.Setup(repo => repo.GetUserByUsernameAsync("unknown"))
            .ReturnsAsync((User)null);

        var authService = new AuthService(mockUserRepo.Object, encryptionService, mockConfig.Object);

        // Act
        var result = await authService.AuthenticateAsync("unknown", "password");

        // Assert
        Assert.Null(result);
    }
}