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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/api/minimal", () => "API Minimal funcionando correctamente.");


app.WebAppCustomConfiguration();

using (var scope = app.Services.CreateScope())
{
    var rolService = scope.ServiceProvider.GetRequiredService<IRolservice>();
    await rolService.SeedDefaultRolesAsync();
    // Esto crea automáticamente Student, Lender y SuperAdmin si no existen
}

app.Run();
