using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO.Auth
{
    public class RegisterDoctorByAdminDto
    {
        // USER AUTH FIELDS
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$", ErrorMessage = "Password must contain at least one uppercase letter and one non-alphanumeric character.")]
        public string Password { get; set; }

        // PERSONAL INFO
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "LicenseNumber is required.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "YearsOfExperience is required.")]
        public int YearsOfExperience { get; set; }

        // RELATIONSHIPS
        [Required(ErrorMessage = "At least one specialization must be selected.")]
        public List<Guid> SpecializationIds { get; set; }

        // ADDRESS INFO
        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }
    }
}
