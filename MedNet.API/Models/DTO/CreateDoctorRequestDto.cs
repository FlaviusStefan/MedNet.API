using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;
using MedNet.API.Attributes;

namespace MedNet.API.Models.DTO
{
    public class CreateDoctorRequestDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [MaxLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; }
        
        public string UserId { get;set; }

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

        [Required(ErrorMessage = "At least one qualification is required.")]
        [MinLength(1, ErrorMessage = "At least one qualification is required.")]
        public ICollection<CreateQualificationDto> Qualifications { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; }

        [Required(ErrorMessage = "At least one specialization is required.")]
        [MinLength(1, ErrorMessage = "At least one specialization is required.")]
        public ICollection<Guid> SpecializationIds { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$", ErrorMessage = "Password must contain an uppercase letter and a special character.")]
        public string Password { get; set; }
    }
}
