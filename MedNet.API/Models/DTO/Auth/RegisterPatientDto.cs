using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;
using MedNet.API.Attributes;

namespace MedNet.API.Models.DTO.Auth
{
    public class RegisterPatientDto
    {
        // USER AUTH FIELDS
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$", ErrorMessage = "Password must contain at least one uppercase letter and one non-alphanumeric character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // PERSONAL INFO
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number.")]
        public string PhoneNumber { get; set; }

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

        // ADDRESS INFO
        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }
    }
}
