namespace MedNet.API.Models.DTO
{
    public class DisplayMedicalFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
