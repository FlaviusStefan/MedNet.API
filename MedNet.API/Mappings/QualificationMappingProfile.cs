using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class QualificationMappingProfile : Profile
    {
        public QualificationMappingProfile()
        {
            CreateMap<Qualification, QualificationDto>();
            CreateMap<Qualification, QualificationResponseDto>();
        }
    }
}