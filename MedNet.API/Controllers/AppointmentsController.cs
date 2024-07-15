using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            this.appointmentRepository = appointmentRepository;
            this.doctorRepository = doctorRepository;
            this.patientRepository = patientRepository;
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

            var response = await MapAppointmentToDto(appointment);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await appointmentRepository.GetAllAsync();
            var response = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                var appointmentDto = await MapAppointmentToDto(appointment);
                response.Add(appointmentDto);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
        {
            var appointment = await appointmentRepository.GetById(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentDto = await MapAppointmentToDto(appointment);
            return Ok(appointmentDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, UpdateAppointmentRequestDto request)
        {
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

            var response = await MapAppointmentToDto(appointment);
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        {
            var appointment = await appointmentRepository.DeleteAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var response = await MapAppointmentToDto(appointment);
            return Ok(response);
        }

        private async Task<AppointmentDto> MapAppointmentToDto(Appointment appointment)
        {
            var doctor = await doctorRepository.GetById(appointment.DoctorId);
            var patient = await patientRepository.GetById(appointment.PatientId);

            var appointmentDto = new AppointmentDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorFirstName = doctor?.FirstName,
                DoctorLastName = doctor?.LastName,
                PatientId = appointment.PatientId,
                PatientFirstName = patient?.FirstName,
                PatientLastName = patient?.LastName,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Reason = appointment.Reason
            };

            return appointmentDto;
        }
    }
}
