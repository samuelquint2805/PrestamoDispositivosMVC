using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using AutoMapper;

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
        }
       

        private static void AddServices( WebApplicationBuilder builder)
        {
            //Services injection

            builder.Services.AddScoped<IStudentService, StudentService>();
            //builder.Services.AddScoped<IDeviceManagerService, DeviceManagerService>();
            //builder.Services.AddScoped<ILoanService, LoanService>();
            //builder.Services.AddScoped<ILoanEventService, LoanEventService>();
        }
    }
}
