namespace MedNet.API.Models.DTO
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string UserId { get; set; }
        public AddressDto Address { get; set; }
        public ContactDto Contact { get; set; }
        public ICollection<DisplayInsuranceDto> Insurances { get; set; }
        public ICollection<DisplayMedicationDto> Medications { get; set; }
        public ICollection<DisplayMedicalFileDto> MedicalFiles { get; set; }
    }
}
