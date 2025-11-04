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
            CreateMap<deviceManager, deviceManagerDTO>().ReverseMap();
            CreateMap<Loan, LoanDTO>().ReverseMap();
            CreateMap<LoanEvent, LoanEventDTO>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<studentStatus, studentStatusDTO>().ReverseMap();
        
        }
    }
}
