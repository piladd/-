using System.Text;
using Messenger.Security;
using Xunit;

namespace Messenger.Tests.Unit;

public class EncryptionServiceTests
{
    [Fact]
    public void EncryptAndDecrypt_Asymmetric_ShouldReturnOriginalText()
    {
        // Arrange
        var service = new EncryptionService();
        var (publicKey, privateKey) = service.GenerateAsymmetricKeys();
        var originalText = "Secret Message";

        // Act
        var encrypted = service.EncryptWithPublicKey(originalText, publicKey);
        var decrypted = service.DecryptWithPrivateKey(encrypted, privateKey);

        // Assert
        Assert.Equal(originalText, decrypted);
    }

    [Fact]
    public void EncryptAndDecrypt_Symmetric_ShouldReturnOriginalText()
    {
        // Arrange
        var service = new EncryptionService();
        var originalText = "Another Secret Message";
        var key = service.GenerateSymmetricKey();

        // Act
        var (iv, encrypted) = service.EncryptWithAes(originalText, key);
        var decrypted = service.DecryptWithAes(encrypted, key, iv);

        // Assert
        Assert.Equal(originalText, decrypted);
    }

    [Fact]
    public void EncryptAndDecrypt_PrivateKey_WithMasterKey_ShouldSucceed()
    {
        // Arrange
        var service = new EncryptionService();
        var (_, privateKey) = service.GenerateAsymmetricKeys();
        var masterKey = "SuperSecretMasterKey123456";

        // Act
        var encrypted = service.EncryptPrivateKey(privateKey, masterKey);
        var decrypted = service.DecryptPrivateKey(encrypted, masterKey);

        // Assert
        Assert.Equal(privateKey, decrypted);
    }
}