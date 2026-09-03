using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            CreateMap<Patient, PatientResponseDto>()
                .ForMember(dest => dest.Insurances, opt => opt.MapFrom(
                    src => src.Insurances))
                .ForMember(dest => dest.MedicalFiles, opt => opt.MapFrom(
                    src => src.MedicalFiles))
                .ForMember(dest => dest.Medications, opt => opt.MapFrom(
                    src => src.Medications));
            CreateMap<Patient, CreatedPatientDto>()
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Contact, opt => opt.Ignore());
            CreateMap<Patient, PatientBasicSummaryDto>();
            CreateMap<Patient, UpdatedPatientDto>();
        }
    }
}