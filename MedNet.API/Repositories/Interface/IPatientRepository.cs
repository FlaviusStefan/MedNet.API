using MedNet.API.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Interface
{
    public interface IPatientRepository
    {
        Task<Patient> CreateAsync(Patient patient);
        Task<IEnumerable<Patient>>GetAllAsync();
        Task<Patient?> GetById(Guid id);
        Task<Patient?> UpdateAsync(Patient patient);
        Task<Patient?> DeleteAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
