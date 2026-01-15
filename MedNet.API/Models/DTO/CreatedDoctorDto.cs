using MedNet.API.Models.Enums;

namespace MedNet.API.Models.DTO
{
    public class CreatedDoctorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public AddressDto Address { get; set; }
        public ContactDto Contact { get; set; }
        public ICollection<string> Specializations { get; set; }
        public ICollection<QualificationDto> Qualifications { get; set; }
    }
}
