using System.Text.Json.Serialization;
using MedNet.API.Models.Enums;

namespace MedNet.API.Models.DTO
{
    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
        public AddressResponseDto Address { get; set; }
        public ContactResponseDto Contact { get; set; }
        public ICollection<DisplayInsuranceDto> Insurances { get; set; }
        public ICollection<DisplayMedicationDto> Medications { get; set; }
        public ICollection<DisplayMedicalFileDto> MedicalFiles { get; set; }
    }
}
