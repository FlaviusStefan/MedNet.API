using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Interface
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
