using MedNet.API.Models.Domain;

namespace MedNet.API.Models.DTO
{
    public class DoctorHospitalDto
    {
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }
    }
}
