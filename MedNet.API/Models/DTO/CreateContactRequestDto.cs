using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateContactRequestDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?!0+$)(\+?\d{1,3}[- ]?)?\d{10,15}$", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}
