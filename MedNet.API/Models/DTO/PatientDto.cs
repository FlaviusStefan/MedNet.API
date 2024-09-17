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
        public AddressDto Address { get; set; }
        public ContactDto Contact { get; set; }
        public ICollection<InsuranceDto> Insurances { get; set; }
        public ICollection<MedicationDto> Medications { get; set; }
    }
}
