using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuración personalizada
builder.AddCustomConfiguration();

// Agregar controladores con vistas
builder.Services.AddControllersWithViews();

// Agregar Notyf 
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

//  Registrar DbContext
builder.Services.AddDbContext<DatacontextPres>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Registrar servicios
builder.Services.AddScoped<ILoanService, LoanService>();

// Registrar servicios auxiliares para 2FA y envío de correo
builder.Services.AddScoped<TwoFactorService>();
builder.Services.AddTransient<SmtpEmailSender>();

// Configurar autenticación por cookie (autenticación manual)
// si quieres usar TempData que depende de cookie-TempDataProvider no requiere configuración adicional

var app = builder.Build();

// Seeder inicial (opcional): crea un admin si no existe
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<DatacontextPres>();

        // Asegurar que la BD existe
        db.Database.EnsureCreated();

        logger.LogInformation("?? Ejecutando seeder de usuarios...");

        // 1?? Crear Admin Principal
        if (!db.Users.Any(u => u.Email == "admin@sistema.com"))
        {
            var admin = new PrestamoDispositivos.Models.ApplicationUser
            {
                UserName = "admin",
                Email = "admin@sistema.com",
                Role = "DeviceManAdmin",
                LockoutEnabled = true,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            db.Users.Add(admin);
            logger.LogInformation("? Usuario admin creado: admin@sistema.com / Admin123!");
        }

        // 2?? Crear Admin Secundario (ejemplo con @admin.gmail.com)
        if (!db.Users.Any(u => u.Email == "supervisor@admin.gmail.com"))
        {
            var supervisor = new PrestamoDispositivos.Models.ApplicationUser
            {
                UserName = "supervisor",
                Email = "supervisor@admin.gmail.com",
                Role = "DeviceManAdmin",
                LockoutEnabled = true,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Super123!")
            };

            db.Users.Add(supervisor);
            logger.LogInformation("? Usuario supervisor creado: supervisor@admin.gmail.com / Super123!");
        }

        // 3?? Crear Estudiantes de Ejemplo
        var estudiantesEjemplo = new[]
        {
            new { UserName = "juan.perez", Email = "juan.perez@estudiante.com", Password = "Juan123!" },
            new { UserName = "maria.lopez", Email = "maria.lopez@estudiante.com", Password = "Maria123!" },
            new { UserName = "carlos.ruiz", Email = "carlos.ruiz@estudiante.com", Password = "Carlos123!" }
        };

        foreach (var est in estudiantesEjemplo)
        {
            if (!db.Users.Any(u => u.Email == est.Email))
            {
                var estudiante = new PrestamoDispositivos.Models.ApplicationUser
                {
                    UserName = est.UserName,
                    Email = est.Email,
                    Role = "Estudiante",
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(est.Password)
                };

                db.Users.Add(estudiante);
                logger.LogInformation($"? Estudiante creado: {est.Email} / {est.Password}");
            }
        }

        // Guardar cambios
        int cambios = db.SaveChanges();

        if (cambios > 0)
        {
            logger.LogInformation($"? Seeder completado: {cambios} usuarios creados.");
        }
        else
        {
            logger.LogInformation("?? No se crearon usuarios (ya existen en BD).");
        }

        // Mostrar resumen en consola
        logger.LogInformation("========================================");
        logger.LogInformation("?? USUARIOS DISPONIBLES:");
        logger.LogInformation("========================================");
        logger.LogInformation("?? ADMINISTRADORES:");
        logger.LogInformation("   - admin@sistema.com / Admin123!");
        logger.LogInformation("   - supervisor@admin.gmail.com / Super123!");
        logger.LogInformation("????? ESTUDIANTES:");
        logger.LogInformation("   - juan.perez@estudiante.com / Juan123!");
        logger.LogInformation("   - maria.lopez@estudiante.com / Maria123!");
        logger.LogInformation("   - carlos.ruiz@estudiante.com / Carlos123!");
        logger.LogInformation("========================================");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "? Error en el seeder inicial: {Message}", ex.Message);
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Autenticación (cookie)
app.UseAuthentication();
app.UseAuthorization();

//  Habilitar Notyf
app.UseNotyf();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.WebAppCustomConfiguration();

app.Run();