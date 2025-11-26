using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// 
// 1) CONFIGURACIÓN PERSONALIZADA (DB, SERVICIOS, COOKIE)
// 
builder.AddCustomConfiguration();

// 
// 2) CONTROLLERS + VISTAS (CON AUTORIZACIÓN GLOBAL)
// 
builder.Services.AddControllersWithViews();

//  AUTORIZACIÓN GLOBAL
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()   // OBLIGA login en TODA la app
        .Build();
});

// 
// 3) NOTYF
// 
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

// 
// 4) SEEDER INICIAL (CREA ADMIN, SUPERVISOR, ESTUDIANTES)
// 
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<DatacontextPres>();

        db.Database.EnsureCreated();
        logger.LogInformation("Ejecutando seeder inicial...");

        // ADMIN PRINCIPAL
        if (!db.Users.Any(u => u.Email == "admin@sistema.com"))
        {
            var admin = new PrestamoDispositivos.Models.ApplicationUser
            {
                UserName = "admin",
                Email = "admin@sistema.com",
                Role = "DeviceManAdmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                LockoutEnabled = true,
            };

            db.Users.Add(admin);
        }

        // ADMIN SECUNDARIO
        if (!db.Users.Any(u => u.Email == "supervisor@admin.gmail.com"))
        {
            var supervisor = new PrestamoDispositivos.Models.ApplicationUser
            {
                UserName = "supervisor",
                Email = "supervisor@admin.gmail.com",
                Role = "DeviceManAdmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Super123!"),
                LockoutEnabled = true,
            };

            db.Users.Add(supervisor);
        }

        // ESTUDIANTES
        var estudiantesEjemplo = new[]
        {
            ("juan.perez",  "juan.perez@estudiante.com",  "Juan123!"),
            ("maria.lopez", "maria.lopez@estudiante.com", "Maria123!"),
            ("carlos.ruiz", "carlos.ruiz@estudiante.com", "Carlos123!")
        };

        foreach (var (user, email, pass) in estudiantesEjemplo)
        {
            if (!db.Users.Any(u => u.Email == email))
            {
                db.Users.Add(new PrestamoDispositivos.Models.ApplicationUser
                {
                    UserName = user,
                    Email = email,
                    Role = "Estudiante",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(pass),
                    LockoutEnabled = true
                });
            }
        }

        db.SaveChanges();
        logger.LogInformation("Seeder completado.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error ejecutando el seeder inicial.");
    }
}

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


var app = builder.Build();

// 
// MIDDLEWARE
// 

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// AUTENTICACIÓN + AUTORIZACIÓN
app.UseAuthentication();
app.UseAuthorization();

//
// ENDPOINTS
// 
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

//
// CONFIGURACIÓN FINAL DE LA APP (Notyf, etc.)
// 
app.WebAppCustomConfiguration();

app.Run();
