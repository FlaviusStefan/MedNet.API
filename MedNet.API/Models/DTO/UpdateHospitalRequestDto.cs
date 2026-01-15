using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdateHospitalRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; }
    }
}
