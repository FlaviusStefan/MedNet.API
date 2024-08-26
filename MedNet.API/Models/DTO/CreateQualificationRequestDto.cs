using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateQualificationRequestDto
    {
        [Required(ErrorMessage = "Doctor ID is required.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Institution is required.")]
        public string Institution { get; set; }

        [Required(ErrorMessage = "Studied years are required.")]
        public int StudiedYears { get; set; }

        [Required(ErrorMessage = "Year of Completion is required.")]
        public int YearOfCompletion { get; set; }
    }
}
