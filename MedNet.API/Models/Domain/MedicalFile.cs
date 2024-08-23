namespace MedNet.API.Models.Domain
{
    public class MedicalFile
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
