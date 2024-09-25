using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdateLabTestRequestDto
    {
        public string TestName { get; set; }
        public string Result { get; set; }
        public string Units { get; set; }
        public string ReferenceRange { get; set; }
    }
}
