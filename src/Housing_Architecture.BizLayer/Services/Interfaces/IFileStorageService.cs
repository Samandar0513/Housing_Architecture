using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(string bucketName, IFormFile file);
    Task<bool> RemoveFileAsync(string bucketName, string objectName);
    Task<Stream?> GetFileAsync(string bucketName, string objectName);
}
