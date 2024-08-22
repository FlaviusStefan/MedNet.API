namespace MedNet.API.Models.Domain
{
    public class DoctorSpecialization
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
    }
}
