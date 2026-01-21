using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Property;
using System.ComponentModel;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IPropertyService
{
    ResponseModel<PropertyDTO> CreateProperty(int userId, PropertyCreateDTO propertyCreateDTO);
    ResponseModel<PropertyDTO> GetPropertyById(int propertyId);
    ResponseModel<PagedResult<PropertyDTO>> GetAllProperties(int pageNumber = 1, int pageSize = 10);
    ResponseModel<IEnumerable<PropertyDTO>> GetAllPropertiesByUserId(int userId);
    ResponseModel<PropertyDTO> UpdateProperty(int propertyId, PropertyUpdateDTO propertyUpdateDTO, int currentUserId);
    ResponseModel<bool> DeleteProperty(int propertyId);
    ResponseModel<bool> TogglePropertyActive(int propertyId, int currentUserId);
}
