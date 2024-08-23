namespace MedNet.API.Models.Domain
{
    public class LabTest
    {
        public Guid Id { get; set; }
        public Guid LabAnalysisId { get; set; }
        public LabAnalysis LabAnalysis { get; set; }
        public string TestName { get; set; }
        public string Result { get; set; }
        public string Units { get; set; }
        public string ReferenceRange { get; set; }
    }
}
