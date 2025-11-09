using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using System;


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

            //manejo de priv8ilegios
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.LoginPath = "/deviceManager/Login";
         options.LogoutPath = "/deviceManager/Logout";
         options.AccessDeniedPath = "/deviceManager/AccessDenied";
     });

            builder.Services.AddAuthorization(options =>
            {
                
                options.AddPolicy("DeviceManagerAdmin", policy => policy.RequireRole("DeviceManAdmin"));
                options.AddPolicy("StudentOnly", policy => policy.RequireRole("Estudiante"));
            });

            // Auto mapper
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            ////Cookie settings
            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Account/Denied";
            //});

            //toast notification
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 7;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });

            //self services
            AddServices(builder);
            return builder;

        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            //Services injection

            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IStudentStatusService, StudentStatusService>();
            builder.Services.AddScoped<IDeviceService, DeviceService>();
            builder.Services.AddScoped<IDeviceManagerService, DeviceManagerService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<ILoanEventService, LoanEventoService>();
        }



        public static WebApplication WebAppCustomConfiguration(this WebApplication app)
        {
               
            

            app.UseAuthentication();
            app.UseAuthorization();

            //  Habilitar Notyf
            app.UseNotyf();
           
            return app;
        }
    }
}
