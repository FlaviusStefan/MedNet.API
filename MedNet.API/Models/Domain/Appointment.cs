namespace MedNet.API.Models.Domain
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }

    }
}
