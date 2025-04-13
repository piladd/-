namespace Messenger.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    // Зашифрованное содержимое файла (например, BLOB, здесь для демонстрации – Base64 представление в виде массива байтов)
    public byte[] EncryptedData { get; set; } = Array.Empty<byte>();
    // Зашифрованный симметричный ключ (Base64)
    public string SymmetricKey { get; set; } = string.Empty;
    // IV для AES (Base64)
    public string IV { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}