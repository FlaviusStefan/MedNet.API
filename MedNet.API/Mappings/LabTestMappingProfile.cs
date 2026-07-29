using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class LabTestMappingProfile : Profile
    {
        public LabTestMappingProfile()
        {
            CreateMap<LabTest, LabTestDto>();
        }
    }
}