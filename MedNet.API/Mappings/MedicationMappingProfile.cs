using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

namespace MedNet.API.Mappings
{
    public class MedicationMappingProfile : Profile
    {
        public MedicationMappingProfile()
        {
            CreateMap<Medication, DisplayMedicationDto>();
        }
    }
}