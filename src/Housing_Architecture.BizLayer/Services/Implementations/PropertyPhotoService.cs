using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class PropertyPhotoService : IPropertyPhotoService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private const string BUCKET_NAME = "property-images";

    public PropertyPhotoService(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ResponseModel<string>> UploadPhotoAsync(int propertyId, IFormFile file)
    {
        var property = _db.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (property == null)
        {
            return ResponseModel<string>.Fail("Xatolik", "E'lon topilmadi!");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return ResponseModel<string>.Fail("Xatolik", "Faqat rasm fayllari qabul qilinadi!");
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return ResponseModel<string>.Fail("Xatolik", "Fayl hajmi 5MB dan oshmasligi kerak!");
        }

        try
        {
            var fileUrl = await _fileStorage.UploadFileAsync(BUCKET_NAME, file);

            var photo = new PropertyPhoto
            {
                PropertyId = propertyId,
                FilePath = fileUrl,
                CreatedAt = DateTime.UtcNow
            };

            _db.PropertyPhotos.Add(photo);
            await _db.SaveChangesAsync();

            return ResponseModel<string>.Ok(fileUrl, "Rasm muvaffaqiyatli yuklandi.");
        }
        catch (Exception ex)
        {
            return ResponseModel<string>.Fail("Xatolik", $"Rasm yuklashda xatolik: {ex.Message}");
        }
    }

    public ResponseModel<bool> DeletePhoto(int photoId)
    {
        var photo = _db.PropertyPhotos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Rasm topilmadi!");
        }

        try
        {
            var uri = new Uri(photo.FilePath);
            var objectName = uri.Segments[^1];
            _fileStorage.RemoveFileAsync(BUCKET_NAME, objectName).Wait();
        }
        catch
        {
            // Log but continue with deletion from database
        }

        _db.PropertyPhotos.Remove(photo);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Rasm muvaffaqiyatli o'chirildi.");
    }

    public ResponseModel<IEnumerable<string>> GetPhotosByPropertyId(int propertyId)
    {
        var photos = _db.PropertyPhotos
            .Where(p => p.PropertyId == propertyId)
            .Select(p => p.FilePath)
            .ToList();

        return ResponseModel<IEnumerable<string>>.Ok(photos, "E'lon rasmlari muvaffaqiyatli olindi.");
    }
}
