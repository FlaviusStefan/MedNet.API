using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateHospitalRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name {  get; set; }


        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressRequestDto Address { get; set; }


        [Required(ErrorMessage = "Contact is required.")]
        public CreateContactRequestDto Contact { get; set; }

    }
}
