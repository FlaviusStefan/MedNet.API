using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;

namespace MedNet.API.Models.Domain
{
    public class Doctor
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public Gender Gender { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; }
        
        [Required]
        [Range(0, 100)]
        public int YearsOfExperience { get; set; }
        
        // One-to-One relationships
        [Required]
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        
        [Required]
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
        
        // Collection navigation properties
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
        public ICollection<DoctorHospital> DoctorHospitals { get; set; } = new List<DoctorHospital>();
        public ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
