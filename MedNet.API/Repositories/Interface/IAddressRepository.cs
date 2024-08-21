using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IAddressRepository
    {
        Task<Address> CreateAsync(Address address);
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address?> GetById(Guid id);
        Task<Address?> UpdateAsync(Address address);
        Task<Address?> DeleteAsync(Guid id);
    }
}
