using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class LabAnalysisMappingProfile : Profile
    {
        public LabAnalysisMappingProfile()
        {
            CreateMap<LabTest, DisplayLabTestDto>();

            CreateMap<LabAnalysis, LabAnalysisDto>()
                .ForMember(dest => dest.LabTests, opt => opt.MapFrom(src => src.LabTests));

            CreateMap<LabAnalysis, DisplayLabAnalysisDto>()
                .ForMember(dest => dest.LabTests, opt => opt.MapFrom(src => src.LabTests));

            CreateMap<LabAnalysis, UpdatedLabAnalysisDto>();
        }
    }
}