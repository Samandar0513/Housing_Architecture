using FluentValidation;
using Housing_Architecture.BizLayer.Models.Property;

namespace Housing_Architecture.BizLayer.Models.Validators.Property;

public class PropertyUpdateValidator : AbstractValidator<PropertyUpdateDTO>
{
    public PropertyUpdateValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategoriya tanlanishi shart.");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).WithMessage("Tuman tanlanishi shart.");

        RuleFor(x => x.RegionId)
            .GreaterThan(0).WithMessage("Viloyat tanlanishi shart.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Tavsif kiritilishi shart.")
            .MaximumLength(2000).WithMessage("Tavsif 2000 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Narx 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Aloqa nomi kiritilishi shart.")
            .MaximumLength(100).WithMessage("Aloqa nomi 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Aloqa telefon raqami kiritilishi shart.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Telefon raqam formati noto'g'ri.");

        RuleFor(x => x.TotalArea)
            .GreaterThan(0).WithMessage("Umumiy maydon 0 dan katta bo'lishi kerak.")
            .When(x => x.TotalArea.HasValue);

        RuleFor(x => x.Rooms)
            .GreaterThan(0).WithMessage("Xonalar soni 0 dan katta bo'lishi kerak.")
            .When(x => x.Rooms.HasValue);

        RuleFor(x => x.Floor)
            .GreaterThanOrEqualTo(0).WithMessage("Qavat 0 dan katta yoki teng bo'lishi kerak.")
            .When(x => x.Floor.HasValue);

        RuleFor(x => x.BuiltYear)
            .InclusiveBetween(1900, DateTime.Now.Year + 5)
            .WithMessage("Qurilgan yil 1900 dan hozirgi yilgacha bo'lishi kerak.")
            .When(x => x.BuiltYear.HasValue);

        // RASMLAR VALIDATSIYASI olib tashlandi - PropertyPhotoController orqali boshqariladi

        // AMENITIES VALIDATSIYASI
        RuleFor(x => x.AmenityIds)
            .Must(amenities => amenities == null || amenities.Count <= 50)
            .WithMessage("Maksimal 50 ta qulaylik tanlash mumkin.")
            .When(x => x.AmenityIds != null);

        RuleForEach(x => x.AmenityIds)
            .GreaterThan(0).WithMessage("Qulaylik ID 0 dan katta bo'lishi kerak.")
            .When(x => x.AmenityIds != null);

        // Takrorlanmaydigan Amenity ID lar
        RuleFor(x => x.AmenityIds)
            .Must(BeUniqueIds).WithMessage("Qulaylik ID lari takrorlanmasligi kerak.")
            .When(x => x.AmenityIds != null && x.AmenityIds.Any());
    }

    // URL tekshirish
    private bool BeValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    // ID lar takrorlanmasligini tekshirish
    private bool BeUniqueIds(List<int>? ids)
    {
        if (ids == null || !ids.Any())
            return true;

        return ids.Count == ids.Distinct().Count();
    }
}