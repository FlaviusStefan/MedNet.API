using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IPatientRepository
    {
        Task<Patient> CreateAsync(Patient patient);
        Task<IEnumerable<Patient>>GetAllAsync();
    }
}
