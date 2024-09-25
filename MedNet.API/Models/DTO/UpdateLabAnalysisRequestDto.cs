using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdateLabAnalysisRequestDto
    {
        public DateTime AnalysisDate { get; set; }  
        public string AnalysisType { get; set; }    
        public List<Guid> LabTestIds { get; set; }

    }
}
 