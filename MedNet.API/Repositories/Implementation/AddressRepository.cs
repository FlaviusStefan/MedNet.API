using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AddressRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Address> CreateAsync(Address address)
        {
            await dbContext.Addresses.AddAsync(address);
            await dbContext.SaveChangesAsync();
            return address;
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await dbContext.Addresses.ToListAsync();
        }

        public async Task<Address?> GetById(Guid id)
        {
            return await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == id);

        }

        public Task<Address?> UpdateAsync(Address address)
        {
            throw new NotImplementedException();
        }

        public Task<Address?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
