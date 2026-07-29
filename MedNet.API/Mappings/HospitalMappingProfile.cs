using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class HospitalMappingProfile : Profile
    {
        public HospitalMappingProfile()
        {
            CreateMap<Hospital, HospitalDto>();
            CreateMap<Hospital, HospitalResponseDto>();
        }
    }
}