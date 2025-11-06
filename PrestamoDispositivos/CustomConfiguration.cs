using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using AutoMapper;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

namespace PrestamoDispositivos
{
    public static class CustomConfiguration
    {

        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder)
        {

            //Data context
            builder.Services.AddDbContext<DatacontextPres>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Auto mapper


            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            //self services
            AddServices(builder);
            return builder;

            //toast notification
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 7;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });

        }




        private static void AddServices(WebApplicationBuilder builder)
        {
            //Services injection

            builder.Services.AddScoped<IStudentService, StudentService>();
            //builder.Services.AddScoped<IDeviceManagerService, DeviceManagerService>();
            //builder.Services.AddScoped<ILoanService, LoanService>();
            //builder.Services.AddScoped<ILoanEventService, LoanEventService>();
        }

        public static WebApplication WebAppCustomConfiguration(this WebApplication app)
        {
            //  Habilitar Notyf
            app.UseNotyf();
           
            return app;
        }
    }
}
