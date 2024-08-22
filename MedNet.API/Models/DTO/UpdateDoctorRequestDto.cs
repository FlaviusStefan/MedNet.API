namespace MedNet.API.Models.DTO
{
    public class UpdateDoctorRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public List<Guid> SpecializationIds { get; set; }


    }
}
