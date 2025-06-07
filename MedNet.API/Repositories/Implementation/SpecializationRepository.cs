using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MedNet.API.Repositories.Implementation
{
    public class SpecializationRepository :ISpecializationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SpecializationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Specialization> CreateAsync(Specialization specialization)
        {
            await dbContext.Specializations.AddAsync(specialization);
            await dbContext.SaveChangesAsync();

            return specialization;
        }

        public async Task<IEnumerable<Specialization>> GetAllAsync()
        {
            return await dbContext.Specializations.ToListAsync();
        }

        public async Task<Specialization?> GetById(Guid id)
        {
            return await dbContext.Specializations.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<Specialization>> GetAllByDoctorIdAsync(Guid doctorId)
        {
            return await dbContext.DoctorSpecializations
                .Where(ds => ds.DoctorId == doctorId)
                .Select(ds => ds.Specialization)
                .ToListAsync();
        }

        public async Task<Specialization?> UpdateAsync(Specialization specialization)
        {
            var existingSpecialization = await dbContext.Specializations.FirstOrDefaultAsync(x => x.Id == specialization.Id);

            if (existingSpecialization != null)
            {
                dbContext.Entry(existingSpecialization).CurrentValues.SetValues(specialization);
                await dbContext.SaveChangesAsync();
                return specialization;
            }

            return null;
        }
        public async Task<Specialization?> DeleteAsync(Guid id)
        {
            var existingSpecialization = await dbContext.Specializations.FirstOrDefaultAsync(x => x.Id == id);

            if (existingSpecialization is null)
            {
                return null;
            }

            dbContext.Specializations.Remove(existingSpecialization);
            await dbContext.SaveChangesAsync();
            return existingSpecialization;
        }
    }
}
