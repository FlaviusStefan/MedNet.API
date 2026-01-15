using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;

namespace MedNet.API.Models.Domain
{
    public class Patient
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(450)]  // Standard AspNetUsers.Id length
        public string UserId { get; set; }  // Reference to AspNetUsers.Id (string)
        
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
        [Range(0, 300)]  // Height in cm
        public double Height { get; set; }
        
        [Required]
        [Range(0, 500)]  // Weight in kg
        public double Weight { get; set; }
        
        // One-to-One relationships
        [Required]
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        
        [Required]
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
        
        // Collection navigation properties
        public ICollection<MedicalFile> MedicalFiles { get; set; } = new List<MedicalFile>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<LabAnalysis> LabAnalyses { get; set; } = new List<LabAnalysis>();
    }
}
