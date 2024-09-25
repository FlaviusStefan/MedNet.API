using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdatedLabAnalysisDto
    {
        public Guid Id { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisType { get; set; }

        [Required]
        public List<Guid> LabTestIds { get; set; } 

    }
}
