using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateInsuranceRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Provider is required.")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Policy Number is required.")]
        public string PolicyNumber { get; set; }

        [Required(ErrorMessage = "Coverage START Date is required.")]
        public DateTime CoverageStartDate { get; set; }

        [Required(ErrorMessage = "Coverage END Date is required.")]
        public DateTime CoverageEndDate { get; set; }
    }
}
