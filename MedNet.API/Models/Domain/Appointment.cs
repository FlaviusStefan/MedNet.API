namespace MedNet.API.Models.Domain
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }

    }
}
