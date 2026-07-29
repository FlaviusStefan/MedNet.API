namespace MedNet.API.Models.Domain
{
    public class LabAnalysis
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisType { get; set; }
        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    }
}
