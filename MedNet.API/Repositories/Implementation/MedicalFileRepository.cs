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
            return medicalFile;
        }

        public async Task<IEnumerable<MedicalFile>> GetAllAsync()
        {
            return await dbContext.MedicalFiles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalFile?> GetById(Guid id)
        {
            return await dbContext.MedicalFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<MedicalFile>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await dbContext.MedicalFiles
                .AsNoTracking()
                .Where(mf => mf.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<MedicalFile?> UpdateAsync(MedicalFile medicalFile)
        {
            var existingMedicalFile = await dbContext.MedicalFiles
                .FirstOrDefaultAsync(x => x.Id == medicalFile.Id);

            if (existingMedicalFile is null)
            {
                return null;
            }

            existingMedicalFile.FileName = medicalFile.FileName;
            existingMedicalFile.FileType = medicalFile.FileType;
            existingMedicalFile.FilePath = medicalFile.FilePath;
            existingMedicalFile.DateUploaded = medicalFile.DateUploaded;

            return existingMedicalFile;
        }

        public async Task<MedicalFile?> DeleteAsync(Guid id)
        {
            var existingMedicalFile = await dbContext.MedicalFiles.FirstOrDefaultAsync(x => x.Id == id);

            if(existingMedicalFile is null)
            {
                return null;
            }

            dbContext.MedicalFiles.Remove(existingMedicalFile);
            return existingMedicalFile;
        }
    }
}