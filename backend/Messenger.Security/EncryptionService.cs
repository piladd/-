using System.Security.Cryptography;
using System.Text;

namespace Messenger.Security
{
    public class EncryptionService
    {
        // Генерация RSA-ключей
        public (byte[] publicKey, byte[] privateKey) GenerateAsymmetricKeys()
        {
            using var rsa = RSA.Create(2048);
            return (rsa.ExportRSAPublicKey(), rsa.ExportRSAPrivateKey());
        }

        // Шифрование данных с помощью публичного ключа
        public byte[] EncryptWithPublicKey(string plainText, byte[] publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);
            var data = Encoding.UTF8.GetBytes(plainText);
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        // Расшифровка данных с помощью приватного ключа
        public string DecryptWithPrivateKey(byte[] encryptedData, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            var decrypted = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decrypted);
        }

        // Генерация случайного симметричного ключа
        public byte[] GenerateSymmetricKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return aes.Key;
        }

        // Симметричное шифрование (AES)
        public (byte[] iv, byte[] ciphertext) EncryptWithAes(string plainText, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return (iv, cipherBytes);
        }

        // Симметричная расшифровка
        public string DecryptWithAes(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
