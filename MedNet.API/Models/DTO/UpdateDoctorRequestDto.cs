using MedNet.API.Models.Enums;
using MedNet.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class UpdateDoctorRequestDto
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
        
        [Required(ErrorMessage = "License Number is required.")]
        [MaxLength(50, ErrorMessage = "License Number cannot exceed 50 characters.")]
        public string LicenseNumber { get; set; }
        
        [Required(ErrorMessage = "Years of experience is required.")]
        [Range(0, 100, ErrorMessage = "Years of experience must be between 0 and 100.")]
        public int YearsOfExperience { get; set; }
        
        [Required(ErrorMessage = "At least one specialization is required.")]
        [MinLength(1, ErrorMessage = "At least one specialization is required.")]
        public List<Guid> SpecializationIds { get; set; }
    }
}
