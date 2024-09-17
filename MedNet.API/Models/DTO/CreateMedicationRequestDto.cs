using MedNet.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateMedicationRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        public string Frequency { get; set; }

    }
}
