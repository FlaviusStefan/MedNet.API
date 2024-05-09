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
    }
}
