using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MedNet.API.Models.DTO
{
    public class UpdatedLabAnalysisDto
    {
        public Guid Id { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisType { get; set; }

    }
}
