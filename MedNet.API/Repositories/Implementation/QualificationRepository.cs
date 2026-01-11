using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public QualificationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Qualification> CreateAsync(Qualification qualification)
        {
            await dbContext.Qualifications.AddAsync(qualification);
            return qualification;
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await dbContext.Qualifications
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Qualification?> GetById(Guid id)
        {
            return await dbContext.Qualifications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Qualification>> GetAllByDoctorIdAsync(Guid doctorId)
        {
            return await dbContext.Qualifications
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<Qualification?> UpdateAsync(Qualification qualification)
        {
            var existingQualification = await dbContext.Qualifications.FirstOrDefaultAsync(x => x.Id == qualification.Id);

            if (existingQualification != null)
            {
                dbContext.Entry(existingQualification).CurrentValues.SetValues(qualification);
                await dbContext.SaveChangesAsync();
                return qualification;
            }

            return null;
        }
        public async Task<Qualification?> DeleteAsync(Guid id)
        {
            var existingQualification = await dbContext.Qualifications.FirstOrDefaultAsync(x => x.Id == id);

            if (existingQualification is null)
            {
                return null;
            }

            dbContext.Qualifications.Remove(existingQualification);
            await dbContext.SaveChangesAsync();
            return existingQualification;
        }
    }
}
