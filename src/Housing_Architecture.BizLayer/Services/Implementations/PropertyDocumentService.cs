using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.PropertyDocument;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Housing_Architecture.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class PropertyDocumentService : IPropertyDocumentService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private const string BUCKET_NAME = "property-documents";

    public PropertyDocumentService(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ResponseModel<PropertyDocumentDTO>> UploadDocumentAsync(int propertyId, IFormFile file)
    {
        var property = _db.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (property == null)
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Mulk topilmadi!");
        }

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Bu fayl formati qabul qilinmaydi!");
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Fayl hajmi 10MB dan oshmasligi kerak!");
        }

        try
        {
            var fileUrl = await _fileStorage.UploadFileAsync(BUCKET_NAME, file);

            var document = new PropertyDocument
            {
                PropertyId = propertyId,
                FileName = file.FileName,
                FilePath = fileUrl,
                Status = DocumentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _db.PropertyDocuments.Add(document);
            await _db.SaveChangesAsync();

            var documentDTO = new PropertyDocumentDTO
            {
                Id = document.Id,
                PropertyId = document.PropertyId,
                FileName = document.FileName,
                FilePath = document.FilePath,
                Status = document.Status.ToString(),
                RejectionReason = document.RejectionReason,
                CreatedAt = document.CreatedAt
            };

            return ResponseModel<PropertyDocumentDTO>.Ok(documentDTO, "Hujjat muvaffaqiyatli yuklandi.");
        }
        catch (Exception ex)
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", $"Hujjat yuklashda xatolik: {ex.Message}");
        }
    }

    public ResponseModel<PropertyDocumentDTO> GetDocumentById(int documentId)
    {
        var document = _db.PropertyDocuments.FirstOrDefault(d => d.Id == documentId);
        if (document == null)
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Hujjat topilmadi!");
        }

        var documentDTO = new PropertyDocumentDTO
        {
            Id = document.Id,
            PropertyId = document.PropertyId,
            FileName = document.FileName,
            FilePath = document.FilePath,
            Status = document.Status.ToString(),
            RejectionReason = document.RejectionReason,
            CreatedAt = document.CreatedAt
        };

        return ResponseModel<PropertyDocumentDTO>.Ok(documentDTO, "Hujjat muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<PropertyDocumentDTO>> GetDocumentsByPropertyId(int propertyId)
    {
        var documents = _db.PropertyDocuments
            .Where(d => d.PropertyId == propertyId)
            .ToList();

        var documentDTOs = documents.Select(d => new PropertyDocumentDTO
        {
            Id = d.Id,
            PropertyId = d.PropertyId,
            FileName = d.FileName,
            FilePath = d.FilePath,
            Status = d.Status.ToString(),
            RejectionReason = d.RejectionReason,
            CreatedAt = d.CreatedAt
        });

        return ResponseModel<IEnumerable<PropertyDocumentDTO>>.Ok(documentDTOs, "Mulk hujjatlari muvaffaqiyatli olindi.");
    }

    public ResponseModel<bool> DeleteDocument(int documentId)
    {
        var document = _db.PropertyDocuments.FirstOrDefault(d => d.Id == documentId);
        if (document == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Hujjat topilmadi!");
        }

        try
        {
            var uri = new Uri(document.FilePath);
            var objectName = uri.Segments[^1];
            _fileStorage.RemoveFileAsync(BUCKET_NAME, objectName).Wait();
        }
        catch
        {
            // Log but continue with deletion from database
        }

        _db.PropertyDocuments.Remove(document);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Hujjat muvaffaqiyatli o'chirildi.");
    }

    public ResponseModel<PropertyDocumentDTO> UpdateDocumentStatus(int documentId, string status, string? rejectionReason = null)
    {
        var document = _db.PropertyDocuments.FirstOrDefault(d => d.Id == documentId);
        if (document == null)
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Hujjat topilmadi!");
        }

        if (!Enum.TryParse<DocumentStatus>(status, true, out var documentStatus))
        {
            return ResponseModel<PropertyDocumentDTO>.Fail("Xatolik", "Noto'g'ri status!");
        }

        document.Status = documentStatus;

        // Rad etilgan bo'lsa, sababni saqlash
        if (documentStatus == DocumentStatus.Rejected)
        {
            document.RejectionReason = rejectionReason;
        }
        else
        {
            document.RejectionReason = null;
        }

        _db.Update(document);
        _db.SaveChanges();

        var documentDTO = new PropertyDocumentDTO
        {
            Id = document.Id,
            PropertyId = document.PropertyId,
            FileName = document.FileName,
            FilePath = document.FilePath,
            Status = document.Status.ToString(),
            RejectionReason = document.RejectionReason,
            CreatedAt = document.CreatedAt
        };

        return ResponseModel<PropertyDocumentDTO>.Ok(documentDTO, "Hujjat statusi muvaffaqiyatli yangilandi.");
    }

    // Barcha pending hujjatlarni olish (Moderator uchun)
    public ResponseModel<PagedResult<PropertyDocumentDTO>> GetPendingDocuments(int pageNumber = 1, int pageSize = 10)
    {
        var query = _db.PropertyDocuments
            .Where(d => d.Status == DocumentStatus.Pending)
            .OrderByDescending(d => d.CreatedAt);

        var totalCount = query.Count();

        var documents = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var documentDTOs = documents.Select(d => new PropertyDocumentDTO
        {
            Id = d.Id,
            PropertyId = d.PropertyId,
            FileName = d.FileName,
            FilePath = d.FilePath,
            Status = d.Status.ToString(),
            RejectionReason = d.RejectionReason,
            CreatedAt = d.CreatedAt
        }).ToList();

        var pagedResult = PagedResult<PropertyDocumentDTO>.Create(
            documentDTOs,
            totalCount,
            pageNumber,
            pageSize);

        return ResponseModel<PagedResult<PropertyDocumentDTO>>.Ok(pagedResult, "Tekshiruv kutayotgan hujjatlar.");
    }
}
