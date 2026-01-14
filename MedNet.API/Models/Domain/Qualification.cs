using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.Domain
{
    public class Qualification
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [MaxLength(200)]
        public string Degree { get; set; }

        [Required]
        [MaxLength(200)]
        public string Institution { get; set; }
        
        [Required]
        [Range(1,15)]
        public int StudiedYears { get; set; }

        [Range(1950, 2100)]
        public int YearOfCompletion { get; set; }
    }
}