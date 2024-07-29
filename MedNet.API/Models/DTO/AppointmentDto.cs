namespace MedNet.API.Models.DTO
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        public Guid PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }

    }
}
