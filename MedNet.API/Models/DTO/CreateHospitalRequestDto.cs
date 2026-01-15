using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateHospitalRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name {  get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }


        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; }

    }
}
