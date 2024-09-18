using MedNet.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.DTO
{
    public class CreateMedicalFileRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "File Name is required.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File Type is required.")]
        public string FileType { get; set; }

        [Required(ErrorMessage = "File Path is required.")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "Uploaded date is required.")]
        public DateTime DateUploaded { get; set; }
    }
}
