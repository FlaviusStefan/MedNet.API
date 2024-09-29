using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using System.Numerics;

namespace MedNet.API.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorService doctorService;
        private readonly IPatientService patientService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorService doctorService, IPatientService patientService)
        {
            this.appointmentRepository = appointmentRepository;
            this.doctorService = doctorService;
            this.patientService = patientService;
        }


        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentRequestDto request)
        {
            var doctor = await doctorService.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new ArgumentException("Invalid DoctorId");
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new ArgumentException("Invalid PatientId");
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                Date = request.Date,
                Status = request.Status,
                Reason = request.Reason,
                Details = request.Details
            };

            var createdAppointment = await appointmentRepository.CreateAsync(appointment);

            return new AppointmentDto
            {
                Id = createdAppointment.Id,
                DoctorId = createdAppointment.DoctorId,
                PatientId = createdAppointment.PatientId,
                Date = createdAppointment.Date,
                Status = createdAppointment.Status,
                Reason = createdAppointment.Reason,
                Details = createdAppointment.Details
            };
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await appointmentRepository.GetAllAsync();

            var doctorDtos = await doctorService.GetAllDoctorsAsync();
            var patientDtos = await patientService.GetAllPatientsAsync();

            return appointments.Select(appointment => new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Details = appointment.Details,
            });
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await appointmentRepository.GetById(id);

            if (appointment == null) return null;


            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Details = appointment.Details,
            };
        }

        public async Task<AppointmentDto?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto request)
        {
            var existingAppointment = await appointmentRepository.GetById(id);
            if(existingAppointment == null) return null;

            existingAppointment.Date = request.Date;
            existingAppointment.Status = request.Status;
            existingAppointment.Reason = request.Reason;
            existingAppointment.Details = request.Details;

            var updatedAppointment = await appointmentRepository.UpdateAsync(existingAppointment);

            if (updatedAppointment == null) return null;

            return new AppointmentDto
            {
                Id = updatedAppointment.Id,
                Date = updatedAppointment.Date,
                Status = updatedAppointment.Status,
                Reason = updatedAppointment.Reason,
                Details = updatedAppointment.Details,
            };
        }
        public async Task<AppointmentDto?> DeleteAppointmentAsync(Guid id)
        {
            var appointment = await appointmentRepository.DeleteAsync(id);
            if(appointment == null) return null;

            return new AppointmentDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Details = appointment.Details,
            };
        }
    }
}
