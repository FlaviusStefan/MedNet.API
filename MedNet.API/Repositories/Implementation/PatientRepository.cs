﻿using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Numerics;

namespace MedNet.API.Repositories.Implementation
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PatientRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            await dbContext.Patients.AddAsync(patient);
            await dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await dbContext.Patients
                .AsNoTracking()
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .ToListAsync();
        }

        public async Task<Patient?> GetById(Guid id)
        {
            return await dbContext.Patients
                .AsNoTracking()
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .Include(p => p.Insurances)
                .Include(p => p.Medications)
                .Include(p => p.MedicalFiles)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Patient?> GetByUserIdAsync(string userId)
        {
            var formattedUserId = userId.Trim();
            return await dbContext.Patients
                .AsNoTracking()
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .Include(p => p.Insurances)
                .Include(p => p.Medications)
                .Include(p => p.MedicalFiles)
                .FirstOrDefaultAsync(x => x.UserId == formattedUserId);
        }

        public async Task<Patient?> UpdateAsync(Patient patient)
        {
            var existingPatient = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == patient.Id);

            if (existingPatient != null)
            {
                dbContext.Entry(existingPatient).CurrentValues.SetValues(patient);
                await dbContext.SaveChangesAsync();
                return patient;
            }

            return null;
        }

        public async Task<Patient?> DeleteAsync(Guid id)
        {
            var existingPatient = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);

            if(existingPatient is null)
            {
                return null;
            }

            dbContext.Patients.Remove(existingPatient);
            await dbContext.SaveChangesAsync();
            return existingPatient;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
    }
}
