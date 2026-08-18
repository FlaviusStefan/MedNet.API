using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class InsuranceMappingProfile : Profile
    {
        public InsuranceMappingProfile()
        { 
            CreateMap<Insurance, InsuranceDto>();
            CreateMap<Insurance, DisplayInsuranceDto>();
        }
    }
}