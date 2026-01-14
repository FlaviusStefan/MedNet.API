using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdateSpecializationRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(200, ErrorMessage = "Name must be at most 200 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters.")]
        public string Description { get; set; }
    }
}
