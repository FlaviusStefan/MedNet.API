using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class LabAnalysisMappingProfile : Profile
    {
        public LabAnalysisMappingProfile()
        {
            CreateMap<LabAnalysis, LabAnalysisDto>();
        }
    }
}