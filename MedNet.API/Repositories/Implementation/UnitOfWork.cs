using MedNet.API.Data;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
