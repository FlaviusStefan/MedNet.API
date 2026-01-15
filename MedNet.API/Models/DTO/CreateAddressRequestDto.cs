using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateAddressRequestDto
    {
        [Required(ErrorMessage = "Street is required.")]
        [MaxLength(200, ErrorMessage = "Street cannot exceed 200 characters.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Street Number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Street Number must be a positive number.")]
        public int StreetNr { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [MaxLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [MaxLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters.")]
        public string PostalCode { get; set; }
    }
}
