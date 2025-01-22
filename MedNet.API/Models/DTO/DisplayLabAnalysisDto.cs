namespace MedNet.API.Models.DTO
{
    public class DisplayLabAnalysisDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisType { get; set; }
        public ICollection<DisplayLabTestDto> LabTests { get; set; }
    }
}
