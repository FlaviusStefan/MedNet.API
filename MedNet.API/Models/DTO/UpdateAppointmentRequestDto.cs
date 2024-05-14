namespace MedNet.API.Models.DTO
{
    public class UpdateAppointmentRequestDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
