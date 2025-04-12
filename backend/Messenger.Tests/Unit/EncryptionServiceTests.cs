using System.Text;
using Messenger.Security;
using Xunit;

namespace Messenger.Tests.Unit;

public class EncryptionServiceTests
{
    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalText()
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
}