using Microsoft.EntityFrameworkCore;
using TechMove.Models;
using TechMove.Services;

namespace TechMove
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<ExchangeRateApiOptions>(
                builder.Configuration.GetSection(ExchangeRateApiOptions.SectionName));

            builder.Services.AddHttpClient<IExchangeRateServices, ExchangeRateService>();
            builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();
            builder.Services.AddSingleton<IFileValidator, FileValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Clients}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
