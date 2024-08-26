using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IQualificationRepository
    {
        Task<Qualification> CreateAsync(Qualification qualification);
        Task<IEnumerable<Qualification>> GetAllAsync();
        Task<Qualification?> GetById(Guid id);
        Task<Qualification?> UpdateAsync(Qualification qualification);
        Task<Qualification?> DeleteAsync(Guid id);
    }
}
