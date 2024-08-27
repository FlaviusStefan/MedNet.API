namespace MedNet.API.Models.Domain
{
    public class DoctorHospital
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Guid HospitalId { get; set; }
        public Hospital Hospital { get; set; }
    }
}
