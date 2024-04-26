namespace MedNet.API.Models.DTO
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Reason { get; set; }
    }
}
