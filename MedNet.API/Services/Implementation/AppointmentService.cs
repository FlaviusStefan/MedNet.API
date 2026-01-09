using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace MedNet.API.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorService doctorService;
        private readonly IPatientService patientService;
        private readonly ILogger<AppointmentService> logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorService doctorService, IPatientService patientService, ILogger<AppointmentService> logger)
        {
            this.appointmentRepository = appointmentRepository;
            this.doctorService = doctorService;
            this.patientService = patientService;
            this.logger = logger;
        }


        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentRequestDto request)
        {
            logger.LogInformation("Creating appointment for Patient {PatientId} with Doctor {DoctorId} on {Date}",
                request.PatientId, request.DoctorId, request.Date);

            var doctor = await doctorService.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                logger.LogWarning("Appointment creation failed - Invalid DoctorId: {DoctorId}", request.DoctorId);
                throw new ArgumentException("Invalid DoctorId");
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                logger.LogWarning("Appointment creation failed - Invalid PatientId: {PatientId}", request.PatientId);
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

            logger.LogInformation("Appointment {AppointmentId} created successfully - Patient: {PatientId}, Doctor: {DoctorId}, Status: {Status}",
                createdAppointment.Id, createdAppointment.PatientId, createdAppointment.DoctorId , createdAppointment.Status);

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
            logger.LogInformation("Retrieving all appointments");

            var appointments = await appointmentRepository.GetAllAsync();

            var doctorDtos = await doctorService.GetAllDoctorsAsync();
            var patientDtos = await patientService.GetAllPatientsAsync();

            var appointmentList = appointments.Select(appointment => new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Details = appointment.Details,
            }).ToList();

            logger.LogInformation("Retrieved {Count} appointments", appointmentList.Count);

            return appointmentList;
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving appointment with ID: {AppointmentId}", id);

            var appointment = await appointmentRepository.GetById(id);

            if (appointment == null)
            {
                logger.LogWarning("Appointment not found with ID: {AppointmentId}", id);
                return null;
            }

            logger.LogInformation("Appointment {AppointmentId} retrieved - Patient: {PatientId}, Doctor: {DoctorId}, Date: {Date}",
                appointment.Id, appointment.PatientId, appointment.DoctorId, appointment.Date);

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
            logger.LogInformation("Updating appointment with ID: {AppointmentId}, New Status: {Status}",
                id, request.Status);

            var existingAppointment = await appointmentRepository.GetById(id);
            if (existingAppointment == null)
            {
                logger.LogWarning("Appointment not found for update with ID: {AppointmentId}", id);
                return null;
            }

            var oldStatus = existingAppointment.Status;

            existingAppointment.Date = request.Date;
            existingAppointment.Status = request.Status;
            existingAppointment.Reason = request.Reason;
            existingAppointment.Details = request.Details;

            var updatedAppointment = await appointmentRepository.UpdateAsync(existingAppointment);

            if (updatedAppointment == null) 
            {
                logger.LogError("Failed to update appointment with ID: {AppointmentId}", id);
                return null;
            }

            logger.LogInformation("Appointment {AppointmentId} updated successfully - Status changed from '{OldStatus}' to '{NewStatus}'",
                id, oldStatus, updatedAppointment.Status);

            return new AppointmentDto
            {
                Id = updatedAppointment.Id,
                Date = updatedAppointment.Date,
                Status = updatedAppointment.Status,
                Reason = updatedAppointment.Reason,
                Details = updatedAppointment.Details,
            };
        }
        public async Task<string?> DeleteAppointmentAsync(Guid id)
        {
            logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);

            var appointment = await appointmentRepository.DeleteAsync(id);
            if (appointment == null)
            {
                logger.LogWarning("Appointment not found for deletion with ID: {AppointmentId}", id);
                return null;
            }

            logger.LogInformation("Appointment {AppointmentId} deleted successfully - Patient: {PatientId}, Doctor: {DoctorId}, Date: {Date}",
                appointment.Id, appointment.PatientId, appointment.DoctorId, appointment.Date);

            return $"Appointment with ID {appointment.Id} deleted successfully!";
        }
    }
}
