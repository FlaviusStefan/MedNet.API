using MedNet.API.Models.Domain;

namespace MedNet.API.Models.DTO
{
    public class LabAnalysisDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisType { get; set; }
        public ICollection<LabTestDto> LabTests { get; set; }
    }
}
