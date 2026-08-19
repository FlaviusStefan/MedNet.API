using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class DoctorMappingProfile : Profile
    {
        public DoctorMappingProfile()
        {
            CreateMap<Doctor, DoctorResponseDto>()
                .ForMember(dest => dest.Specializations, opt => opt.MapFrom(
                    src => src.DoctorSpecializations.Select(ds => ds.Specialization.Name).ToList()))
                .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.Qualifications));

            CreateMap<Doctor, CreatedDoctorDto>()
                .ForMember(dest => dest.Specializations, opt => opt.Ignore())
                .ForMember(dest => dest.Qualifications, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Contact, opt => opt.Ignore());

            CreateMap<Doctor, UpdatedDoctorDto>()
                .ForMember(dest => dest.SpecializationIds, opt => opt.Ignore());
        }
    }
}
