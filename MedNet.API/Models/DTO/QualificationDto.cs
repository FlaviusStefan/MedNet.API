using MedNet.API.Models.Domain;

namespace MedNet.API.Models.DTO
{
    public class QualificationDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string Degree { get; set; }
        public string Institution { get; set; }
        public int StudiedYears { get; set; }
        public int YearOfCompletion { get; set; }
    }
}
