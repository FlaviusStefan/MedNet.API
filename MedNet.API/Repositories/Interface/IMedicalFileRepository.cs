using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IMedicalFileRepository
    {
        Task<MedicalFile> CreateAsync(MedicalFile medicalFile);
        Task<IEnumerable<MedicalFile>> GetAllAsync();
        Task<MedicalFile?> GetById(Guid id);
        Task<MedicalFile?> UpdateAsync(MedicalFile medicalFile);
        Task<MedicalFile?> DeleteAsync(Guid id);
    }
}
