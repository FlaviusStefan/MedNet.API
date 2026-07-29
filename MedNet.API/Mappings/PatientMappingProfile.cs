using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            CreateMap<Patient, PatientResponseDto>();
            CreateMap<Patient, CreatedPatientDto>();
            CreateMap<Patient, PatientBasicSummaryDto>();
            CreateMap<Patient, UpdatedPatientDto>();
        }
    }
}