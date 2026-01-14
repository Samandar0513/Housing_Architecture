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
            .When(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name != "string");

        RuleFor(x => x.Phone)
            .Matches(@"^(?:\+998|998)?(90|91|93|94|95|97|98|99|33|88)\d{7}$")
            .WithMessage("Telefon raqam formati noto'g'ri.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone) && x.Phone != "string");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email formati noto'g'ri.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email) && x.Email != "string");
    }
}
