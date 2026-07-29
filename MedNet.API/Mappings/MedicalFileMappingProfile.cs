using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class MedicalFileMappingProfile : Profile
    {
        public MedicalFileMappingProfile()
        {
            CreateMap<MedicalFile, DisplayMedicalFileDto>();
        }
    }
}