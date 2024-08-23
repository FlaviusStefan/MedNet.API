namespace MedNet.API.Models.DTO
{
    public class UpdateAppointmentRequestDto
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }

    }
}
