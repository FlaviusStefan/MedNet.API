namespace MedNet.API.Models.Domain
{
    public class Medication
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
