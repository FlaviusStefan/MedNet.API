using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class DoctorHospitalRepository : IDoctorHospitalRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DoctorHospitalRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task BindAsync(DoctorHospital doctorHospital)
        {
            await dbContext.DoctorHospitals.AddAsync(doctorHospital);
            await dbContext.SaveChangesAsync();
        }

        public async Task UnbindAsync(Guid doctorId, Guid hospitalId)
        {
            var doctorHospital = await dbContext.DoctorHospitals
                .FirstOrDefaultAsync(dh => dh.DoctorId == doctorId && dh.HospitalId == hospitalId);

            if (doctorHospital != null)
            {
                dbContext.DoctorHospitals.Remove(doctorHospital);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<DoctorHospital?> GetBindingAsync(Guid doctorId, Guid hospitalId)
        {
            return await dbContext.DoctorHospitals.FirstOrDefaultAsync(dh => dh.DoctorId == doctorId && dh.HospitalId == hospitalId);

        }

        public async Task<IEnumerable<DoctorHospital>> GetDoctorsByHospitalAsync(Guid hospitalId)
        {
            return await dbContext.DoctorHospitals
                .Where(dh => dh.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorHospital>> GetHospitalsByDoctorAsync(Guid doctorId)
        {
            return await dbContext.DoctorHospitals
                .Where(dh => dh.DoctorId == doctorId)
                .ToListAsync();
        }
    }

}
