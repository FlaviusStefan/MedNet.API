using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AppointmentRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await dbContext.Appointments.AddAsync(appointment);
            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await dbContext.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Appointment?> GetById(Guid id)
        {
            return await dbContext.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment?> UpdateAsync(Appointment appointment)
        {
            var existingAppointment = await dbContext.Appointments
                .FirstOrDefaultAsync(x => x.Id == appointment.Id);

            if (existingAppointment is null)
            {
                return null;
            }

            existingAppointment.Date = appointment.Date;
            existingAppointment.Status = appointment.Status;
            existingAppointment.Reason = appointment.Reason;
            existingAppointment.Details = appointment.Details;

            return existingAppointment;
        }

        public async Task<Appointment?> DeleteAsync(Guid id)
        {
            var existingAppointment = await dbContext.Appointments
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if(existingAppointment is null) 
            {
                return null;
            }

            dbContext.Appointments.Remove(existingAppointment);
            return existingAppointment;
        }
    }
}