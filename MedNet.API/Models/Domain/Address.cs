using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.Domain
{
    public class Address
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Street { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int StreetNr { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string City { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string State { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Country { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }
    }
}
