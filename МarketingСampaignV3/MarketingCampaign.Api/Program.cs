
using MarketingCampaign.Api.Services;
using MarketingCampaign.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace MarketingCampaign.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ƒобавл€ю свои сервис в DI-контейнер.
            builder.Services.AddScoped<CampaignValidator>();
            builder.Services.AddScoped<DiscountSelector>();
            builder.Services.AddScoped<TokenService>();

            // JWT аутентификаци€
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });


            builder.Services.AddAuthorization();

            //—канируем сборку и регистируем в DI-контейнере контроллеры, 
            //а также методы дл€ сериализации/десериализации JSON вместе с конвертарами, 
            //которые позвол€ют enum'ы сериализовать текстом, а не цифрами.
            builder.Services.AddControllers()
                            .AddJsonOptions(o => o.JsonSerializerOptions.Converters
                            .Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

            //подготовливаем базу метеданных по всем контроллерам в том числе дл€ Swagger-документации
            builder.Services.AddEndpointsApiExplorer();

            // Ќастройка Swagger с поддержкой JWT
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketingCampaign API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "¬ведите JWT токен (без слова 'Bearer', оно добавитс€ автоматически)."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


            var app = builder.Build();

            //internalization
            //var supportedCultures = new[] { "ru", "en" };
            //app.UseRequestLocalization(new RequestLocalizationOptions()
            //    .SetDefaultCulture("ru")
            //    .AddSupportedCultures(supportedCultures)
            //    .AddSupportedUICultures(supportedCultures));

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //с помощью данного middleware http-запросы переправл€ютс€ на https
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            //найденные endpoints выстраиваем в pipeline маршрутизацию
            app.MapControllers();

            app.Run();
        }
    }
}
