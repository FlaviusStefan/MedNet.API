using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class DoctorHospitalMappingProfile : Profile
    {
        public DoctorHospitalMappingProfile()
        {
            CreateMap<DoctorHospital, DoctorHospitalDto>();
        }
    }
}
