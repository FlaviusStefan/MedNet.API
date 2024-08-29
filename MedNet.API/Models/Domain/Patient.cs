namespace MedNet.API.Models.Domain
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
        public ICollection<MedicalFile> MedicalFiles { get; set; } = new List<MedicalFile>();
        public ICollection<Medication> CurrentMedications { get; set; } = new List<Medication>();
        public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<LabAnalysis> LabAnalyses { get; set; } = new List<LabAnalysis>();
    }
}
