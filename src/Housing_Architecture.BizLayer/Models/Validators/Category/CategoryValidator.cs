using FluentValidation;
using Housing_Architecture.BizLayer.Models.Category;

namespace Housing_Architecture.BizLayer.Models.Validators.Category;

public class CategoryValidator : AbstractValidator<CategoryCreateDTO>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategoriya nomi kiritilishi shart.")
            .MaximumLength(100).WithMessage("Kategoriya nomi 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Tavsif 500 ta belgidan oshmasligi kerak.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
