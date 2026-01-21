using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.PropertyDocument;
using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IPropertyDocumentService
{
    Task<ResponseModel<PropertyDocumentDTO>> UploadDocumentAsync(int propertyId, IFormFile file);
    ResponseModel<PropertyDocumentDTO> GetDocumentById(int documentId);
    ResponseModel<IEnumerable<PropertyDocumentDTO>> GetDocumentsByPropertyId(int propertyId);
    ResponseModel<bool> DeleteDocument(int documentId);
    ResponseModel<PropertyDocumentDTO> UpdateDocumentStatus(int documentId, string status, string? rejectionReason = null);
    ResponseModel<PagedResult<PropertyDocumentDTO>> GetPendingDocuments(int pageNumber = 1, int pageSize = 10);
}
