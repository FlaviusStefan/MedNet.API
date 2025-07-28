using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateDoctorRequestDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        public string UserId { get;set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Years of experience is required.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; }

        [Required(ErrorMessage = "SpecializationIds are required.")]
        public ICollection<Guid> SpecializationIds { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$", ErrorMessage = "Password must contain an uppercase letter and a special character.")]
        public string Password { get; set; }
    }
}
