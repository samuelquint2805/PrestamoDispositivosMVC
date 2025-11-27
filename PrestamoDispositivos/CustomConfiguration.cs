using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using System;

namespace PrestamoDispositivos
{
    public static class CustomConfiguration
    {
      
        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DatacontextPres>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";

                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;

                    options.Cookie.Name = "PrestamoDispositivosAuth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

            // ================
            // C) AUTORIZACIÓN (POLICIES)
            // ================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("DeviceManagerAdmin", policy =>
                    policy.RequireRole("DeviceManAdmin"));

                options.AddPolicy("StudentOnly", policy =>
                    policy.RequireRole("Estudiante"));
            });

            // ================
            // D) AUTOMAPPER
            // ================
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // ================
            // E) TOAST NOTIFICATION
            // ================
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 7;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });

            builder.Services.AddAuthentication()
     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
     {
         var jwtSecret = builder.Configuration["JwtSettings:Secret"];

         // Validar que la clave existe
         if (string.IsNullOrEmpty(jwtSecret))
         {
             throw new InvalidOperationException("JWT Secret no está configurada en appsettings.json");
         }

         // Validar longitud mínima
         if (jwtSecret.Length < 32)
         {
             throw new InvalidOperationException("JWT Secret debe tener al menos 32 caracteres");
         }

         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret)),
             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
             ValidAudience = builder.Configuration["JwtSettings:Audience"],
             ClockSkew = TimeSpan.Zero
         };
     });
            // ================
            // F) SERVICES INJECTION
            // ================
            AddServices(builder);

            return builder;
        }

        // ============================================================
        // 2) REGISTRO DE SERVICIOS (SIN 2FA)
        // ============================================================
        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IStudentStatusService, StudentStatusService>();
            builder.Services.AddScoped<IDeviceService, DeviceService>();
            builder.Services.AddScoped<IDeviceManagerService, DeviceManagerService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<ILoanEventService, LoanEventoService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        }

        // ============================================================
        // 3) CONFIGURACIÓN FINAL DEL PIPELINE — LLAMADO DESDE Program.cs
        // ============================================================
        public static WebApplication WebAppCustomConfiguration(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            // Notyf notifications
            app.UseNotyf();

            return app;
        }
    }
}
