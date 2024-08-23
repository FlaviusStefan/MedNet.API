namespace MedNet.API.Models.Domain
{
    public class Insurance
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime CoverageStartDate { get; set; }
        public DateTime CoverageEndDate { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
