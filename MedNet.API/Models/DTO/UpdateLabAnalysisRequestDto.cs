using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MedNet.API.Models.DTO
{
    public class UpdateLabAnalysisRequestDto
    {
        public DateTime AnalysisDate { get; set; }  
        public string AnalysisType { get; set; }

    }
}
 