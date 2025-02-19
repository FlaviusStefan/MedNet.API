using MedNet.API.Models;

namespace MedNet.API.Repositories.Interface
{
    public interface IInsuranceRepository
    {
        Task<Insurance> CreateAsync(Insurance insurance);
        Task<IEnumerable<Insurance>> GetAllAsync();
        Task<Insurance?> GetById(Guid id);
        Task<IEnumerable<Insurance>> GetAllByPatientIdAsync(Guid patientId);
        Task<Insurance?> UpdateAsync(Insurance insurance);
        Task<Insurance?> DeleteAsync(Guid id);
    }
}
