using MedNet.API.Data;
using MedNet.API.Models;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly ApplicationDbContext dbContext;

        public InsuranceRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Insurance> CreateAsync(Insurance insurance)
        {
            await dbContext.Insurances.AddAsync(insurance);
            await dbContext.SaveChangesAsync();

            return insurance;
        }

        public async Task<IEnumerable<Insurance>> GetAllAsync()
        {
            return await dbContext.Insurances.ToListAsync();
        }

        public async Task<Insurance?> GetById(Guid id)
        {
            return await dbContext.Insurances.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<Insurance>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await dbContext.Insurances
                .Where(i => i.PatientId == patientId)
                .ToListAsync();
        }


        public async Task<Insurance?> UpdateAsync(Insurance insurance)
        {
            var existingInsurance = await dbContext.Insurances.FirstOrDefaultAsync(x => x.Id == insurance.Id);

            if (existingInsurance != null)
            {
                dbContext.Entry(existingInsurance).CurrentValues.SetValues(insurance);
                await dbContext.SaveChangesAsync();
                return insurance;
            }

            return null;
        }
        public async Task<Insurance?> DeleteAsync(Guid id)
        {
            var existingInsurance = await dbContext.Insurances.FirstOrDefaultAsync(x => x.Id == id);

            if (existingInsurance is null)
            {
                return null;
            }

            dbContext.Insurances.Remove(existingInsurance);
            await dbContext.SaveChangesAsync();
            return existingInsurance;

        }
    }
}
