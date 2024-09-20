using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateLabTestRequestDto
    {

        [Required(ErrorMessage = "Test name is required.")]
        public string TestName { get; set; }

        [Required(ErrorMessage = "Result is required.")]
        public string Result { get; set; }

        [Required(ErrorMessage = "Units are required.")]
        public string Units { get; set; }

        [Required(ErrorMessage = "Reference Range is required.")]
        public string ReferenceRange { get; set; }
    }
}
