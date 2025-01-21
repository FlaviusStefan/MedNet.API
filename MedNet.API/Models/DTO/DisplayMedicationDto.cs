namespace MedNet.API.Models.DTO
{
    public class DisplayMedicationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
    }
}
