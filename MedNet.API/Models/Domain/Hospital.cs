using Microsoft.AspNetCore.Identity;

namespace MedNet.API.Models.Domain
{
    public class Hospital
    {
        public Guid Id { get; set; }
        public string? IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        public string Name { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
        public ICollection<DoctorHospital> DoctorHospitals { get; set; } = new List<DoctorHospital>();
    }
}
