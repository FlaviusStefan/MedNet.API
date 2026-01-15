using MedNet.API.Models.Enums;
using MedNet.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdatePatientRequestDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required.")]
        [MaxLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Gender is required.")]
        [ValidEnum(ErrorMessage = "Gender must be either Male or Female.")]
        public Gender Gender { get; set; }
        
        [Required(ErrorMessage = "Height is required.")]
        [Range(0.1, 300.0, ErrorMessage = "Height must be between 0.1 and 300 cm.")]
        public double Height { get; set; }
        
        [Required(ErrorMessage = "Weight is required.")]
        [Range(0.1, 500.0, ErrorMessage = "Weight must be between 0.1 and 500 kg.")]
        public double Weight { get; set; }
    }
}
