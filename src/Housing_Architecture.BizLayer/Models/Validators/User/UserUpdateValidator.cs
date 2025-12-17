using FluentValidation;
using Housing_Architecture.BizLayer.Models.User;

namespace Housing_Architecture.BizLayer.Models.Validators.User;

public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Ism kamida 3 ta belgidan iborat bo'lishi kerak.")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Telefon raqam formati noto'g'ri.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email formati noto'g'ri.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
