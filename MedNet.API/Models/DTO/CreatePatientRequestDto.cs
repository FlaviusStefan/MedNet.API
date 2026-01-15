using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;
using MedNet.API.Attributes;

namespace MedNet.API.Models.DTO
{
    public class CreatePatientRequestDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [ValidEnum(ErrorMessage = "Gender must be either Male or Female.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; } // Contact details will be filled from user registration
    }
}
