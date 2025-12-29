namespace MedNet.API.Models.DTO
{
    public class DoctorResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public AddressResponseDto Address { get; set; }
        public ContactResponseDto Contact { get; set; }
        public ICollection<string> Specializations { get; set; }
        public ICollection<QualificationResponseDto> Qualifications { get; set; }
    }
}
