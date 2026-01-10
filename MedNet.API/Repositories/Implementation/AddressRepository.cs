using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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

        public async Task<Address?> UpdateAsync(Address address)
        {
            var existingAddress = await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == address.Id);

            if (existingAddress != null)
            {
                dbContext.Entry(existingAddress).CurrentValues.SetValues(address);
                await dbContext.SaveChangesAsync();
                return address;
            }

            return null;
        }

        public async Task<Address?> DeleteAsync(Guid id)
        {
            var existingAddress = await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == id);

            if (existingAddress is null)
            {
                return null;
            }

            dbContext.Addresses.Remove(existingAddress);
            await dbContext.SaveChangesAsync();
            return existingAddress;
        }
    }
}
