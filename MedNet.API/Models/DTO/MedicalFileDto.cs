using MedNet.API.Models.Domain;

namespace MedNet.API.Models.DTO
{
    public class MedicalFileDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
