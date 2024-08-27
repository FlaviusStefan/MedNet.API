using MedNet.API.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Interface
{
    public interface IHospitalRepository
    {
        Task<Hospital> CreateAsync(Hospital hospital);
        Task<IEnumerable<Hospital>> GetAllAsync();
        Task<Hospital?> GetById(Guid id);
        Task<Hospital?> UpdateAsync(Hospital hospital);
        Task<Hospital?> DeleteAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
