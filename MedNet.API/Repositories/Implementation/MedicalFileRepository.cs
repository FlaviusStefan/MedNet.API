using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class MedicalFileRepository : IMedicalFileRepository
    {
        private readonly ApplicationDbContext dbContext;

        public MedicalFileRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<MedicalFile> CreateAsync(MedicalFile medicalFile)
        {
            await dbContext.MedicalFiles.AddAsync(medicalFile);
            await dbContext.SaveChangesAsync(); 

            return medicalFile;
        }

        public async Task<IEnumerable<MedicalFile>> GetAllAsync()
        {
            return await dbContext.MedicalFiles.ToListAsync();
        }

        public async Task<MedicalFile?> GetById(Guid id)
        {
            return await dbContext.MedicalFiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MedicalFile?> UpdateAsync(MedicalFile medicalFile)
        {
            var existingMedicalFile = await dbContext.MedicalFiles.FirstOrDefaultAsync(x => x.Id == medicalFile.Id);

            if(existingMedicalFile != null)
            {
                dbContext.Entry(existingMedicalFile).CurrentValues.SetValues(medicalFile);
                await dbContext.SaveChangesAsync();
                return medicalFile;
            }

            return null;
        }

        public async Task<MedicalFile?> DeleteAsync(Guid id)
        {
            var existingMedicalFile = await dbContext.MedicalFiles.FirstOrDefaultAsync(x => x.Id == id);

            if(existingMedicalFile is null)
            {
                return null;
            }

            dbContext.MedicalFiles.Remove(existingMedicalFile);
            await dbContext.SaveChangesAsync();
            return existingMedicalFile;
        }
    }
}