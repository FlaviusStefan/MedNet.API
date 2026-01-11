using MedNet.API.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Interface
{
    public interface IDoctorRepository
    {
        Task<Doctor> CreateAsync(Doctor doctor);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetById(Guid id);
        Task<Doctor?> UpdateAsync(Doctor doctor);
        Task<Doctor?> DeleteAsync(Guid id);
        Task UpdateDoctorSpecializationsAsync(Guid doctorId, IEnumerable<Guid> specializationIds);
    }
}
