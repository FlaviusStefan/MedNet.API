using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IMedicationRepository
    {
        Task<Medication> CreateAsync(Medication medication);
        Task<IEnumerable<Medication>> GetAllAsync();
        Task<Medication?> GetById(Guid id);
        Task<IEnumerable<Medication>> GetAllByPatientIdAsync(Guid patientId);
        Task<Medication?> UpdateAsync(Medication medication);
        Task<Medication?> DeleteAsync(Guid id);
    }
}
