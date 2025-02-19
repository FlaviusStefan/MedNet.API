using MedNet.API.Data;
using MedNet.API.Models;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public MedicationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Medication> CreateAsync(Medication medication)
        {
            await dbContext.Medications.AddAsync(medication);
            await dbContext.SaveChangesAsync();

            return medication;
        }

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await dbContext.Medications.ToListAsync();
        }

        public async Task<Medication?> GetById(Guid id)
        {
            return await dbContext.Medications.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Medication>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await dbContext.Medications
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<Medication?> UpdateAsync(Medication medication)
        {
            var existingMedication = await dbContext.Medications.FirstOrDefaultAsync(x => x.Id == medication.Id);

            if (existingMedication != null)
            {
                dbContext.Entry(existingMedication).CurrentValues.SetValues(medication);
                await dbContext.SaveChangesAsync();
                return medication;
            }

            return null;
        }
        public async Task<Medication?> DeleteAsync(Guid id)
        {
            var existingMedication = await dbContext.Medications.FirstOrDefaultAsync(x => x.Id == id);

            if (existingMedication is null)
            {
                return null;
            }

            dbContext.Medications.Remove(existingMedication);
            await dbContext.SaveChangesAsync();
            return existingMedication;
        }
    }
}
