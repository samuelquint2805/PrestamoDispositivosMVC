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
builder.Services.AddAuthentication("MiCookieAuth")
    .AddCookie("MiCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
    });

// si quieres usar TempData que depende de cookie-TempDataProvider no requiere configuración adicional

var app = builder.Build();

// Seeder inicial (opcional): crea un admin si no existe
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<DatacontextPres>();
        // Crear usuario admin si no existe (simple seeder)
        if (!db.Users.Any(u => u.Email == "admin@yeff.local"))
        {
            var admin = new PrestamoDispositivos.Models.ApplicationUser
            {
                UserName = "admin",
                Email = "admin@yeff.local",
                Role = "DeviceManager",
                LockoutEnabled = true
            };
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error en el seeder inicial.");
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