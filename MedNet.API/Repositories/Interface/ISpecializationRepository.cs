using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface ISpecializationRepository
    {
        Task<Specialization> CreateAsync(Specialization specialization);
        Task<IEnumerable<Specialization>> GetAllAsync();
        Task<Specialization?> GetById(Guid id);
        Task<Specialization?> UpdateAsync(Specialization specialization);
        Task<Specialization?> DeleteAsync(Guid id);
    }
}
