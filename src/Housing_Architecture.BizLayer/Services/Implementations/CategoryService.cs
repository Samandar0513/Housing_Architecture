using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Category;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public ResponseModel<CategoryDTO> CreateCategory(CategoryCreateDTO categoryCreateDTO)
    {
        if (string.IsNullOrWhiteSpace(categoryCreateDTO.Name))
        {
            return ResponseModel<CategoryDTO>.Fail("Xatolik", "Kategoriya nomi kiritilmadi!");
        }

        var category = new Category
        {
            Name = categoryCreateDTO.Name,
            Description = categoryCreateDTO.Description
        };

        _db.Categories.Add(category);
        _db.SaveChanges();

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return ResponseModel<CategoryDTO>.Ok(categoryDTO, "Kategoriya muvaffaqiyatli yaratildi.");
    }

    public ResponseModel<CategoryDTO> GetCategoryById(int categoryId)
    {
        var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
        {
            return ResponseModel<CategoryDTO>.Fail("Xatolik", "Kategoriya topilmadi!");
        }

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return ResponseModel<CategoryDTO>.Ok(categoryDTO, "Kategoriya muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<CategoryDTO>> GetAllCategories()
    {
        var categories = _db.Categories.ToList();
        var categoryDTOs = categories.Select(category => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });

        return ResponseModel<IEnumerable<CategoryDTO>>.Ok(categoryDTOs, "Barcha kategoriyalar muvaffaqiyatli olindi.");
    }

    public ResponseModel<CategoryDTO> UpdateCategory(int categoryId, CategoryCreateDTO categoryUpdateDTO)
    {
        var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
        {
            return ResponseModel<CategoryDTO>.Fail("Xatolik", "Kategoriya topilmadi!");
        }

        if (string.IsNullOrWhiteSpace(categoryUpdateDTO.Name))
        {
            return ResponseModel<CategoryDTO>.Fail("Xatolik", "Yangi kategoriya nomi kiritilmadi!");
        }

        category.Name = categoryUpdateDTO.Name;
        category.Description = categoryUpdateDTO.Description;

        _db.Update(category);
        _db.SaveChanges();

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return ResponseModel<CategoryDTO>.Ok(categoryDTO, "Kategoriya muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteCategory(int categoryId)
    {
        var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Kategoriya topilmadi!");
        }

        _db.Categories.Remove(category);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Kategoriya muvaffaqiyatli o'chirildi.");
    }
}
