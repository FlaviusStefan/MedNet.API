using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository)
        {
            this.appointmentRepository = appointmentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentRequestDto request)
        {
            var appointment = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                AppointmentDateTime = request.AppointmentDateTime,
                Status = request.Status,
                Reason = request.Reason
            };

            await appointmentRepository.CreateAsync(appointment);

            var response = new AppointmentDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Reason = appointment.Reason
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await appointmentRepository.GetAllAsync();

            // Mapping: Domain model to DTO 

            var response = new List<AppointmentDto>();
            foreach (var appointment in appointments)
            {
                response.Add(new AppointmentDto
                {
                    Id = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId,
                    AppointmentDateTime = appointment.AppointmentDateTime,
                    Status = appointment.Status,
                    Reason = appointment.Reason
                });
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
        {
            var existingAppointment = await appointmentRepository.GetById(id);

            if (existingAppointment is null)
            {
                return NotFound();
            }

            var response = new AppointmentDto
            {
                Id = existingAppointment.Id,
                DoctorId = existingAppointment.DoctorId,
                PatientId = existingAppointment.PatientId,
                AppointmentDateTime = existingAppointment.AppointmentDateTime,
                Status = existingAppointment.Status,
                Reason = existingAppointment.Reason
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, UpdateAppointmentRequestDto request)
        {
            // DTO to Domain Model
            var appointment = new Appointment
            {
                Id = id,
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                AppointmentDateTime = request.AppointmentDateTime,
                Status = request.Status,
                Reason = request.Reason
            };

            appointment = await appointmentRepository.UpdateAsync(appointment);

            if (appointment == null)
            {
                return NotFound();
            }

            // Domain Model to DTO
            var response = new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Reason = appointment.Reason
            };

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        {
            var appointment = await appointmentRepository.DeleteAsync(id);

            if (appointment is null)
            {
                return NotFound();
            }

            // Convert Domain Model to DTO

            var response = new AppointmentDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Reason = appointment.Reason

            };

            return Ok(response);
        }
    }
}
