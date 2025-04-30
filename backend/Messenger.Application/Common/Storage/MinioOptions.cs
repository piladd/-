namespace Messenger.Infrastructure.Storage;

/// <summary>
/// Настройки подключения к MinIO (или S3-совместимому хранилищу).
/// </summary>
public class MinioOptions
{
    public string Endpoint { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string BucketName { get; set; } = "attachments";
}