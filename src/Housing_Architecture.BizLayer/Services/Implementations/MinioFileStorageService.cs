using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioFileStorageService> _logger;

    public MinioFileStorageService(
        IMinioClient minioClient,
        IOptions<MinioSettings> settings,
        ILogger<MinioFileStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(string bucketName, IFormFile file)
    {
        try
        {
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }

            var objectName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            var protocol = _settings.UseSsl ? "https" : "http";
            //return $"{protocol}://{_settings.Endpoint}/{bucketName}/{objectName}";
            return $"http://localhost:9000/{bucketName}/{objectName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MinIO");
            throw;
        }
    }

    public async Task<bool> RemoveFileAsync(string bucketName, string objectName)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing file from MinIO: {ObjectName}", objectName);
            return false;
        }
    }

    public async Task<Stream?> GetFileAsync(string bucketName, string objectName)
    {
        try
        {
            var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file from MinIO: {ObjectName}", objectName);
            return null;
        }
    }
}
