using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Category;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface ICategoryService
{
    ResponseModel<CategoryDTO> CreateCategory(CategoryCreateDTO categoryCreateDTO);
    ResponseModel<CategoryDTO> GetCategoryById(int categoryId);
    ResponseModel<IEnumerable<CategoryDTO>> GetAllCategories();
    ResponseModel<CategoryDTO> UpdateCategory(int categoryId, CategoryCreateDTO categoryUpdateDTO);
    ResponseModel<bool> DeleteCategory(int categoryId);
}
