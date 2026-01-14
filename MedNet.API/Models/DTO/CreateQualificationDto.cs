using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateQualificationDto
    {
        [Required(ErrorMessage = "Degree is required.")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Institution is required.")]
        public string Institution { get; set; }

        [Required(ErrorMessage = "Studied years are required.")]
        [Range(1, 15, ErrorMessage = "Studied years must be between 1 and 15.")]
        public int StudiedYears { get; set; }

        [Required(ErrorMessage = "Year of Completion is required.")]
        [Range(1950, 2100, ErrorMessage = "Year of Completion must be between 1950 and 2100.")]
        public int YearOfCompletion { get; set; }
    }
}
