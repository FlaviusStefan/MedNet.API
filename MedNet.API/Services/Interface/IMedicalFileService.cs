using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IMedicalFileService
    {
        Task<MedicalFileDto> CreateMedicalFileAsync(CreateMedicalFileRequestDto request);
        Task<IEnumerable<MedicalFileDto>> GetAllMedicalFilesAsync();
        Task<MedicalFileDto?> GetMedicalFileByIdAsync(Guid id);
        Task<IEnumerable<MedicalFileDto>> GetMedicalFilesByPatientIdAsync(Guid patientId);
        Task<MedicalFileDto> UpdateMedicalFileAsync(Guid id, UpdateMedicalFileRequestDto request);
        Task<MedicalFileDto> DeleteMedicalFileAsync(Guid id);
    }
}
