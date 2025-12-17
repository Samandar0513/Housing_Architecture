using FluentValidation;
using Housing_Architecture.BizLayer.Models.User;

namespace Housing_Architecture.BizLayer.Models.Validators.User;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDTO>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Eski parol kiritilishi shart.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yangi parol kiritilishi shart.")
            .MinimumLength(6).WithMessage("Yangi parol kamida 6 ta belgidan iborat bo'lishi kerak.")
            .Matches(@"[A-Z]").WithMessage("Yangi parolda kamida 1 ta katta harf bo'lishi kerak.")
            .Matches(@"[a-z]").WithMessage("Yangi parolda kamida 1 ta kichik harf bo'lishi kerak.")
            .Matches(@"[0-9]").WithMessage("Yangi parolda kamida 1 ta raqam bo'lishi kerak.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Parolni tasdiqlash kiritilishi shart.")
            .Equal(x => x.NewPassword).WithMessage("Parollar mos kelmadi.");
    }
}
