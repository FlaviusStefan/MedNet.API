using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.Domain
{
    public class Hospital
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        // One-to-One relationships
        [Required]
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        
        [Required]
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
        
        // Collection navigation properties
        public ICollection<DoctorHospital> DoctorHospitals { get; set; } = new List<DoctorHospital>();
    }
}
