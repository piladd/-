using System.Security.Cryptography;
using System.Text;

namespace Messenger.Security;

/// <summary>
/// Сервис для симметричного и асимметричного шифрования данных (AES и RSA).
/// </summary>
public class EncryptionService
{
    /// <summary>
    /// Генерирует пару RSA-ключей: публичный и приватный.
    /// </summary>
    public (byte[] publicKey, byte[] privateKey) GenerateAsymmetricKeys()
    {
        using var rsa = RSA.Create(2048);
        return (rsa.ExportRSAPublicKey(), rsa.ExportRSAPrivateKey());
    }

    /// <summary>
    /// Шифрует строку с использованием публичного RSA-ключа.
    /// </summary>
    public byte[] EncryptWithPublicKey(string plainText, byte[] publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        var data = Encoding.UTF8.GetBytes(plainText);
        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    /// <summary>
    /// Расшифровывает данные с помощью приватного RSA-ключа.
    /// </summary>
    public string DecryptWithPrivateKey(byte[] encryptedData, byte[] privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        var decrypted = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decrypted);
    }

    /// <summary>
    /// Генерирует симметричный AES-ключ.
    /// </summary>
    public byte[] GenerateSymmetricKey()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        return aes.Key;
    }

    /// <summary>
    /// Шифрует строку с использованием симметричного ключа AES.
    /// </summary>
    /// <returns>IV и зашифрованные байты</returns>
    public (byte[] iv, byte[] ciphertext) EncryptWithAes(string plainText, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return (iv, cipherBytes);
    }

    /// <summary>
    /// Расшифровывает AES-данные по ключу и IV.
    /// </summary>
    public string DecryptWithAes(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var decryptedBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    /// <summary>
    /// Шифрует приватный RSA-ключ с использованием мастер-ключа.
    /// </summary>
    /// <remarks>Приватный ключ сохраняется как строка в формате: base64(IV):base64(cipher)</remarks>
    public string EncryptPrivateKey(byte[] privateKey, string masterKey)
    {
        using var aes = Aes.Create();
        aes.Key = GetKeyFromMaster(masterKey);
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var encryptedBytes = encryptor.TransformFinalBlock(privateKey, 0, privateKey.Length);
        return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Расшифровывает приватный RSA-ключ с использованием мастер-ключа.
    /// </summary>
    public byte[] DecryptPrivateKey(string encryptedPrivateKey, string masterKey)
    {
        var parts = encryptedPrivateKey.Split(':');
        if (parts.Length != 2)
            throw new Exception("Неверный формат зашифрованного ключа.");

        var iv = Convert.FromBase64String(parts[0]);
        var encryptedBytes = Convert.FromBase64String(parts[1]);

        using var aes = Aes.Create();
        aes.Key = GetKeyFromMaster(masterKey);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
    }

    /// <summary>
    /// Получает ключ из мастер-ключа путём хеширования с SHA256.
    /// </summary>
    private byte[] GetKeyFromMaster(string masterKey)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(masterKey));
    }
}
