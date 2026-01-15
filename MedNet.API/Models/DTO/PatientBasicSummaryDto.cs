using MedNet.API.Models.Enums;

namespace MedNet.API.Models.DTO
{
    public class PatientBasicSummaryDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public AddressResponseDto Address { get; set; }
        public ContactResponseDto Contact { get; set; }
    }
}
