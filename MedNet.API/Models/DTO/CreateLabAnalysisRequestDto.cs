using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateLabAnalysisRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Analysis Date is required.")]
        public DateTime AnalysisDate { get; set; }

        [Required(ErrorMessage = "Analysis Type is required.")]
        public string AnalysisType { get; set; }

        [Required(ErrorMessage = "Lab Test is required.")]
        public ICollection<CreateLabTestDto> LabTests { get; set; }
    }
}
