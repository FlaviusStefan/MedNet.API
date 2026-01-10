using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Implementation
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly ApplicationDbContext dbContext;

        public HospitalRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Hospital> CreateAsync(Hospital hospital)
        {
            await dbContext.Hospitals.AddAsync(hospital);
            return hospital;
        }

        public async Task<IEnumerable<Hospital>> GetAllAsync()
        {
            return await dbContext.Hospitals
                .Include(h => h.Address)
                .Include(h => h.Contact)
                .ToListAsync();

        }

        public async Task<Hospital?> GetById(Guid id)
        {
            return await dbContext.Hospitals
                .Include(h => h.Address)
                .Include(h => h.Contact)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Hospital?> UpdateAsync(Hospital hospital)
        {
            var existingHospital = await dbContext.Hospitals.FirstOrDefaultAsync(x => x.Id == hospital.Id);

            if (existingHospital != null)
            {
                dbContext.Entry(existingHospital).CurrentValues.SetValues(hospital);
                await dbContext.SaveChangesAsync();
                return hospital;
            }

            return null;
        }
        public async Task<Hospital?> DeleteAsync(Guid id)
        {
            var existingHospital = await dbContext.Hospitals.FirstOrDefaultAsync(x => x.Id == id);

            if (existingHospital is null)
            {
                return null;
            }

            dbContext.Hospitals.Remove(existingHospital);
            await dbContext.SaveChangesAsync();
            return existingHospital;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
    }
}
