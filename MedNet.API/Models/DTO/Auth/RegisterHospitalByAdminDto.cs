using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO.Auth
{
    public class RegisterHospitalByAdminDto
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

        // PERSONAL INFO
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number.")]
        public string PhoneNumber { get; set; }

        // ADDRESS INFO
        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }
    }
}
