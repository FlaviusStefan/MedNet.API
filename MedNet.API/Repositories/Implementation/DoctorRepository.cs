using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;

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
    }
}
