using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IDoctorRepository
    {
        Task<Doctor> CreateAsync(Doctor doctor);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetById(Guid id);
        Task<Doctor?> UpdateAsync(Doctor doctor);
        Task<Doctor?> DeleteAsync(Guid id);
    }
}
