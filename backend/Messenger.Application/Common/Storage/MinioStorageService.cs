using Minio;
using Minio.DataModel.Args;
using Messenger.Application.Common.Storage;
using Microsoft.Extensions.Options;
using System.Text;

namespace Messenger.Infrastructure.Storage;

/// <summary>
/// Сервис для работы с MinIO хранилищем.
/// </summary>
public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioStorageService(IMinioClient minioClient, IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(string fileName, byte[] content)
    {
        await EnsureBucketExistsAsync();

        var objectName = $"{Guid.NewGuid()}_{fileName}";

        using var stream = new MemoryStream(content);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType("application/octet-stream");

        await _minioClient.PutObjectAsync(putObjectArgs);

        return objectName;
    }

    public async Task<byte[]?> DownloadAsync(string objectName)
    {
        try
        {
            using var ms = new MemoryStream();

            var getArgs = new GetObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(ms));

            await _minioClient.GetObjectAsync(getArgs);
            return ms.ToArray();
        }
        catch (MinioException)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string objectName)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeArgs);
    }

    public async Task<bool> ExistsAsync(string objectName)
    {
        try
        {
            var statArgs = new StatObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectName);

            await _minioClient.StatObjectAsync(statArgs);
            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }

    private async Task EnsureBucketExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_options.BucketName);

        bool exists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
        if (!exists)
        {
            var makeArgs = new MakeBucketArgs()
                .WithBucket(_options.BucketName);

            await _minioClient.MakeBucketAsync(makeArgs);
        }
    }
}
