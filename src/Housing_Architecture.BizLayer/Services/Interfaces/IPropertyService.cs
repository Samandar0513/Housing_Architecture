using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Property;
using System.ComponentModel;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IPropertyService
{
    ResponseModel<PropertyDTO> CreateProperty(int userId, PropertyCreateDTO propertyCreateDTO);
    ResponseModel<PropertyDTO> GetPropertyById(int propertyId);
    ResponseModel<IEnumerable<PropertyDTO>> GetAllProperties();
    ResponseModel<IEnumerable<PropertyDTO>> GetAllPropertiesByUserId(int userId);
    ResponseModel<PropertyDTO> UpdateProperty(int propertyId, PropertyUpdateDTO propertyUpdateDTO, int currentUserId);
    ResponseModel<bool> DeleteProperty(int propertyId);
}
