using Housing_Architecture.BizLayer;
using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Services;
using Housing_Architecture.DataAccess;
using Housing_Architecture.DataAccess.Persistence;
using Microsoft.OpenApi.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Housing_Architecture.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Data Access Layer
        builder.Services.AddDataAccess(configuration);

        // Business Layer
        builder.Services.AddBizLayer(configuration);

        // Telegram Bot
        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = configuration["TelegramBot:Token"];
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Telegram Bot Token is not configured");
            return new TelegramBotClient(token);
        });
        builder.Services.AddHostedService<TelegramBotService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        builder.Services.AddEndpointsApiExplorer();

        // Swagger with JWT support
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Housing Architecture API",
                Version = "v1",
                Description = "Ko'chmas mulk boshqaruv tizimi API"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token kiriting: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("ProductionCorsPolicy", policy =>
            {
                policy.WithOrigins(
                        "https://housing.example.com",
                        "https://www.housing.example.com"
                    )
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials();
            });
        });

        //var helper = new Housing_Architecture.BizLayer.Common.Helper();
        //var salt = Guid.NewGuid().ToString();
        //var password = helper.Encript("Saman0513", salt);

        //var user = new Housing_Architecture.Domain.Entities.User();
        //user.Name = "Samandar";
        //user.Email = "samandar8821@mail.ru";
        //user.Phone = "+998937110513";
        //user.Salt = salt;
        //user.Role = "Admin";
        //user.Password = "hashed_password";
        //user.IsVerified = true;
        //var db = new AppDbContext();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("CorsPolicy");
        }
        else
        {
            app.UseCors("ProductionCorsPolicy");
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
