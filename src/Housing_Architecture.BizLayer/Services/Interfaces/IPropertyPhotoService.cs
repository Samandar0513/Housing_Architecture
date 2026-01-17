using Housing_Architecture.BizLayer.Models;
using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IPropertyPhotoService
{
    Task<ResponseModel<string>> UploadPhotoAsync(int propertyId, IFormFile file);
    ResponseModel<bool> DeletePhoto(int photoId);
    ResponseModel<IEnumerable<PhotoDto>> GetPhotosByPropertyId(int propertyId);
}
