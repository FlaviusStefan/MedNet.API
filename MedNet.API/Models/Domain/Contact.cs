using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.Domain
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Patient> Patients { get; set; }
    }
}
