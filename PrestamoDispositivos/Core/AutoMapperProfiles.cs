using AutoMapper;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.Core
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            // CreateMap<Source, Destination>();
            // Example:
            // CreateMap<Student, StudentDTO>();
            CreateMap<Device, deviceDTO>().ReverseMap();
            CreateMap<Administrator, AdministratorDTO>().ReverseMap();
            CreateMap<Loan, LoanDTO>().ReverseMap();
            CreateMap<AuditReportsClass, AuditReportClassDTO>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<lender, lenderDTO >().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDTO>().ReverseMap();
            CreateMap<Request, RequestoDTO>().ReverseMap();
            CreateMap<setRol, setRolDTO>().ReverseMap();


        }
    }
}
