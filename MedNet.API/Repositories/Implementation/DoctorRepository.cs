﻿using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Repositories.Implementation
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DoctorRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Doctor> CreateAsync(Doctor doctor)
        {
            await dbContext.Doctors.AddAsync(doctor);
            await dbContext.SaveChangesAsync();

            return doctor;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await dbContext.Doctors
                .Include(d => d.Address)
                .Include(d => d.Contact)
                .Include(d => d.DoctorSpecializations)
                .ThenInclude(d => d.Specialization)
                .ToListAsync();
        }

        public async Task<Doctor?> GetById(Guid id)
        {
            return await dbContext.Doctors
                .Include(d => d.Address)
                .Include(d => d.Contact)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(d => d.Specialization)
                .Include(d => d.Qualifications)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Doctor?> UpdateAsync(Doctor doctor)
        {
            var existingDoctor = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == doctor.Id);

            if (existingDoctor != null)
            {
                dbContext.Entry(existingDoctor).CurrentValues.SetValues(doctor);
                await dbContext.SaveChangesAsync();
                return doctor;
            }

            return null;
        }

        public async Task<Doctor?> DeleteAsync(Guid id)
        {
            var existingDoctor = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);

            if(existingDoctor is null)
            {
                return null;
            }

            dbContext.Doctors.Remove(existingDoctor);
            await dbContext.SaveChangesAsync();
            return existingDoctor;
        }

        public async Task UpdateDoctorSpecializationsAsync(Guid doctorId, IEnumerable<Guid> specializationIds)
        {
            var currentSpecializations = dbContext.DoctorSpecializations
                .Where(ds => ds.DoctorId == doctorId)
                .ToList();

            dbContext.DoctorSpecializations.RemoveRange(currentSpecializations);

            var newSpecializations = specializationIds.Select(sid => new DoctorSpecialization
            {
                DoctorId = doctorId,
                SpecializationId = sid
            });

            await dbContext.DoctorSpecializations.AddRangeAsync(newSpecializations);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }

    }
}
