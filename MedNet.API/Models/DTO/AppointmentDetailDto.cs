namespace MedNet.API.Models.DTO
{
    public class AppointmentDetailDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorFullName { get; set; }
        public Guid PatientId { get; set; }
        public string PatientFullName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
    }
}