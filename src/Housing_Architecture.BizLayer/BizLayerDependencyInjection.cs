using FluentValidation;
using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Mapping;
using Housing_Architecture.BizLayer.Models.Validators.Category;
using Housing_Architecture.BizLayer.Models.Validators.Property;
using Housing_Architecture.BizLayer.Models.Validators.User;
using Housing_Architecture.BizLayer.Services;
using Housing_Architecture.BizLayer.Services.Implementations;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

namespace Housing_Architecture.BizLayer;

public static class BizLayerDependencyInjection
{
    public static IServiceCollection AddBizLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddFluentValidations();
        services.AddJwtAuthentication(configuration);

        services.Configure<MinioSettings>(configuration.GetSection("MinioSettings"));
        services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
        services.Configure<JwtOption>(configuration.GetSection("JwtOption"));

        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IPropertyDocumentService, PropertyDocumentService>();
        services.AddScoped<Helper>();

        services.AddScoped<SearchPropertiesService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IAmenityService, AmenityService>();
        services.AddScoped<IPropertyPhotoService, PropertyPhotoService>();

        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IFileStorageService, MinioFileStorageService>();
        services.AddSingleton<IMinioClient>(sp =>
        {
            var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            var client = new MinioClient()
                .WithEndpoint(minioSettings.Endpoint)
                .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey);

            if (minioSettings.UseSsl)
            {
                client = client.WithSSL();
            }

            return client.Build();
        });
    }

    private static void AddFluentValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserRegistrValidator>();
        services.AddValidatorsFromAssemblyContaining<PropertyCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
        services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOption = new JwtOption();
        configuration.GetSection("JwtOption").Bind(jwtOption);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOption.Issuer,
                ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Key)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }
}
