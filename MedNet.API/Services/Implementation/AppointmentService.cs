using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorService doctorService;
        private readonly IPatientService patientService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<AppointmentService> logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepository, 
            IDoctorService doctorService, 
            IPatientService patientService, 
            ILogger<AppointmentService> logger, 
            IUnitOfWork unitOfWork)
        {
            this.appointmentRepository = appointmentRepository;
            this.doctorService = doctorService;
            this.patientService = patientService;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<CreatedAppointmentDto> CreateAppointmentAsync(CreateAppointmentRequestDto request)
        {
            logger.LogInformation(
                "Creating appointment for Patient {PatientId} with Doctor {DoctorId} on {Date}",
                request.PatientId, request.DoctorId, request.Date);

            if (request.Date < DateTime.UtcNow)
            {
                logger.LogWarning("Appointment creation failed - Date is in the past: {Date}", request.Date);
                throw new ArgumentException("Appointment date cannot be in the past");
            }

            var doctor = await doctorService.GetDoctorByIdAsync(request.DoctorId);
            if (doctor is null)
            {
                logger.LogWarning("Appointment creation failed - Invalid DoctorId: {DoctorId}", request.DoctorId);
                throw new ArgumentException($"Doctor with ID {request.DoctorId} not found");
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient is null)
            {
                logger.LogWarning("Appointment creation failed - Invalid PatientId: {PatientId}", request.PatientId);
                throw new ArgumentException($"Patient with ID {request.PatientId} not found");
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
            await unitOfWork.SaveChangesAsync(); 

            logger.LogInformation(
                "Appointment {AppointmentId} created successfully - Patient: {PatientId}, Doctor: {DoctorId}, Status: {Status}",
                createdAppointment.Id, createdAppointment.PatientId, createdAppointment.DoctorId, createdAppointment.Status);

            return new CreatedAppointmentDto
            {
                Id = createdAppointment.Id,
                DoctorId = createdAppointment.DoctorId,
                DoctorFullName = $"{doctor.FirstName} {doctor.LastName}",
                PatientId = createdAppointment.PatientId,
                PatientFullName = $"{patient.FirstName} {patient.LastName}",
                Date = createdAppointment.Date,
                Status = createdAppointment.Status,
                Reason = createdAppointment.Reason,
                Details = createdAppointment.Details,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<AppointmentSummaryDto>> GetAllAppointmentsAsync()
        {
            logger.LogInformation("Retrieving all appointments");

            var appointments = await appointmentRepository.GetAllAsync();

            var appointmentList = appointments.Select(appointment => new AppointmentSummaryDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorFullName = appointment.Doctor != null 
                    ? $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}" 
                    : "Unknown Doctor",
                PatientId = appointment.PatientId,
                PatientFullName = appointment.Patient != null 
                    ? $"{appointment.Patient.FirstName} {appointment.Patient.LastName}" 
                    : "Unknown Patient",
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason
            }).ToList();

            logger.LogInformation("Retrieved {Count} appointments", appointmentList.Count);

            return appointmentList;
        }

        public async Task<AppointmentDetailDto?> GetAppointmentByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving appointment with ID: {AppointmentId}", id);

            var appointment = await appointmentRepository.GetById(id);

            if (appointment is null)
            {
                logger.LogWarning("Appointment not found with ID: {AppointmentId}", id);
                return null;
            }

            logger.LogInformation(
                "Appointment {AppointmentId} retrieved - Patient: {PatientId}, Doctor: {DoctorId}, Date: {Date}",
                appointment.Id, appointment.PatientId, appointment.DoctorId, appointment.Date);

            return new AppointmentDetailDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorFullName = appointment.Doctor != null 
                    ? $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}" 
                    : "Unknown Doctor",
                PatientId = appointment.PatientId,
                PatientFullName = appointment.Patient != null 
                    ? $"{appointment.Patient.FirstName} {appointment.Patient.LastName}" 
                    : "Unknown Patient",
                Date = appointment.Date,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Details = appointment.Details
            };
        }

        public async Task<UpdatedAppointmentDto?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto request)
        {
            logger.LogInformation(
                "Updating appointment with ID: {AppointmentId}, New Status: {Status}",
                id, request.Status);

            var existingAppointment = await appointmentRepository.GetById(id);
            
            if (existingAppointment is null)
            {
                logger.LogWarning("Appointment not found for update with ID: {AppointmentId}", id);
                return null;
            }

            if (request.Date < DateTime.UtcNow)
            {
                logger.LogWarning("Appointment update failed - Date is in the past: {Date}", request.Date);
                throw new ArgumentException("Appointment date cannot be in the past");
            }

            var oldStatus = existingAppointment.Status;

            existingAppointment.Date = request.Date;
            existingAppointment.Status = request.Status;
            existingAppointment.Reason = request.Reason;
            existingAppointment.Details = request.Details;

            var updatedAppointment = await appointmentRepository.UpdateAsync(existingAppointment);

            if (updatedAppointment is null) 
            {
                logger.LogError("Failed to update appointment with ID: {AppointmentId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation(
                "Appointment {AppointmentId} updated successfully - Status changed from '{OldStatus}' to '{NewStatus}'",
                id, oldStatus, updatedAppointment.Status);

            return new UpdatedAppointmentDto
            {
                Id = updatedAppointment.Id,
                DoctorId = updatedAppointment.DoctorId,
                PatientId = updatedAppointment.PatientId,
                Date = updatedAppointment.Date,
                Status = updatedAppointment.Status,
                Reason = updatedAppointment.Reason,
                Details = updatedAppointment.Details,
                LastModifiedAt = DateTime.UtcNow
            };
        }

        public async Task<string?> DeleteAppointmentAsync(Guid id)
        {
            logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);

            var appointment = await appointmentRepository.DeleteAsync(id);
            if (appointment is null)
            {
                logger.LogWarning("Appointment not found for deletion with ID: {AppointmentId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation(
                "Appointment {AppointmentId} deleted successfully - Patient: {PatientId}, Doctor: {DoctorId}, Date: {Date}",
                appointment.Id, appointment.PatientId, appointment.DoctorId, appointment.Date);

            return $"Appointment with ID {appointment.Id} deleted successfully!";
        }
    }
}
