using System.ComponentModel.DataAnnotations;
using MedNet.API.Models.Enums;

namespace MedNet.API.Models.DTO
{
    public class UpdatedDoctorDto
    {
        public Guid Id {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }

        [Required]
        public List<Guid> SpecializationIds { get; set; }


    }
}
