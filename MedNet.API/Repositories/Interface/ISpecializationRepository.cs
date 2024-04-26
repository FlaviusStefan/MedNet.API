using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface ISpecializationRepository
    {
        Task<Specialization> CreateAsync(Specialization specialization);
    }
}
