using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreatePatientRequestDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; }
    }
}
