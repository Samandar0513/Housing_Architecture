using FluentValidation;
using Housing_Architecture.BizLayer.Models.User;

namespace Housing_Architecture.BizLayer.Models.Validators.User;

public class UserRegistrValidator : AbstractValidator<UserRegistrDTO>
{
    public UserRegistrValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ism kiritilishi shart.")
            .MinimumLength(3).WithMessage("Ism kamida 3 ta belgidan iborat bo'lishi kerak.")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon raqam kiritilishi shart.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Telefon raqam formati noto'g'ri.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email kiritilishi shart.")
            .EmailAddress().WithMessage("Email formati noto'g'ri.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol kiritilishi shart.")
            .MinimumLength(6).WithMessage("Parol kamida 6 ta belgidan iborat bo'lishi kerak.")
            .Matches(@"[A-Z]").WithMessage("Parolda kamida 1 ta katta harf bo'lishi kerak.")
            .Matches(@"[a-z]").WithMessage("Parolda kamida 1 ta kichik harf bo'lishi kerak.")
            .Matches(@"[0-9]").WithMessage("Parolda kamida 1 ta raqam bo'lishi kerak.");
    }
}
