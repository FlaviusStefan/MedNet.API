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
            return medication;
        }

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await dbContext.Medications
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Medication?> GetById(Guid id)
        {
            return await dbContext.Medications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Medication>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await dbContext.Medications
                .AsNoTracking()
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<Medication?> UpdateAsync(Medication medication)
        {
            var existingMedication = await dbContext.Medications
                .FirstOrDefaultAsync(x => x.Id == medication.Id);

            if (existingMedication is null)
            {
                return null;
            }

            existingMedication.Name = medication.Name;
            existingMedication.Dosage = medication.Dosage;
            existingMedication.Frequency = medication.Frequency;

            return existingMedication;
        }
        public async Task<Medication?> DeleteAsync(Guid id)
        {
            var existingMedication = await dbContext.Medications.FirstOrDefaultAsync(x => x.Id == id);

            if (existingMedication is null)
            {
                return null;
            }

            dbContext.Medications.Remove(existingMedication);
            return existingMedication;
        }
    }
}
