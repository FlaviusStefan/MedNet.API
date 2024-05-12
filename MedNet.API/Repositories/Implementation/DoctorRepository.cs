using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

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
            return await dbContext.Doctors.ToListAsync();
        }

        public async Task<Doctor?> GetById(Guid id)
        {
            return await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
