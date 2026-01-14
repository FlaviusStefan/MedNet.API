using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateQualificationRequestDto
    {
        [Required(ErrorMessage = "Doctor ID is required.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        [StringLength(200, ErrorMessage = "Degree must be at most 200 characters.")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Institution is required.")]
        [StringLength(200, ErrorMessage = "Institution must be at most 200 characters.")]
        public string Institution { get; set; }

        [Required(ErrorMessage = "StudiedYears is required.")]
        [Range(1, 15, ErrorMessage = "StudiedYears must be between 1 and 15.")]
        public int StudiedYears { get; set; }

        [Required(ErrorMessage = "YearOfCompletion is required.")]
        [Range(1950, 2100, ErrorMessage = "YearOfCompletion must be between 1950 and 2100.")]
        public int YearOfCompletion { get; set; }
    }
}
