using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class SpecializationMappingProfile : Profile
    {
        public SpecializationMappingProfile()
        {
            CreateMap<Specialization, SpecializationDto>();
        }
    }
}