using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateAddressRequestDto
    {
        [Required]
        public string Street { get; set; }

        [Required]
        public int StreetNr { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }
}
