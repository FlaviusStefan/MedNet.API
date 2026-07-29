using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Appointment, AppointmentSummaryDto>()
                .ForMember(dest => dest.DoctorFullName, opt => opt.MapFrom(
                    src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : "Unknown Doctor"))
                .ForMember(dest => dest.PatientFullName, opt => opt.MapFrom(
                    src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : "Unknown Patient"));

            CreateMap<Appointment, AppointmentDetailDto>()
                .ForMember(dest => dest.DoctorFullName, opt => opt.MapFrom(
                    src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : "Unknown Doctor"))
                .ForMember(dest => dest.PatientFullName, opt => opt.MapFrom(
                    src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : "Unknown Patient"));

            CreateMap<Appointment, CreatedAppointmentDto>()
                .ForMember(dest => dest.DoctorFullName, opt => opt.MapFrom(
                    src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : "Unknown Doctor"))
                .ForMember(dest => dest.PatientFullName, opt => opt.MapFrom(
                    src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : "Unknown Patient"))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Appointment, UpdatedAppointmentDto>()
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
        }
    }
}