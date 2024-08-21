using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IContactRepository
    {
        Task<Contact> CreateAsync(Contact contact);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact?> GetById(Guid id);
        Task<Contact?> UpdateAsync(Contact contact);
        Task<Contact?> DeleteAsync(Guid id);
    }
}
