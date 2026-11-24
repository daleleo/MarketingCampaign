
using MarketingCampaign.Application;

namespace MarketingCampaign.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the DI-container.
            builder.Services.AddControllers();

            // Add my services to the DI-container.
            builder.Services.AddScoped<CampaignValidator>();
            builder.Services.AddScoped<DiscountSelector>();

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


            //Middleware of internalization: choosing a culture
            var supportedCultures = new[] { "ru", "en" };
            app.UseRequestLocalization(new RequestLocalizationOptions()
                .SetDefaultCulture("ru")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
