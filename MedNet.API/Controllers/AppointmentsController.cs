using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService appointmentService;
        private readonly ILogger<AppointmentsController> logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            this.appointmentService = appointmentService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} creating appointment for Patient {PatientId} with Doctor {DoctorId}",
                userId, userRole, request.PatientId, request.DoctorId);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid appointment creation request from user {UserId}: {ValidationErrors}",
                    userId, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            try
            {
                var response = await appointmentService.CreateAppointmentAsync(request);
                logger.LogInformation("Appointment {AppointmentId} created successfully by user {UserId}",
                    response.Id, userId);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Appointment creation failed for user {UserId}: {Message}", userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while user {UserId} creating appointment", userId);
                return StatusCode(500, "An error occurred while creating the appointment.");
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all appointments", userId, userRole);

            var response = await appointmentService.GetAllAppointmentsAsync();

            logger.LogInformation("Returned {Count} appointments to user {UserId}",
                ((IEnumerable<AppointmentDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting appointment {AppointmentId}",
                userId, userRole, id);

            var response = await appointmentService.GetAppointmentByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Appointment {AppointmentId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, UpdateAppointmentRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} updating appointment {AppointmentId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid appointment update request from user {UserId} for appointment {AppointmentId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await appointmentService.UpdateAppointmentAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Appointment {AppointmentId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Appointment {AppointmentId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} deleting appointment {AppointmentId}",
                userId, userRole, id);

            var response = await appointmentService.DeleteAppointmentAsync(id);

            if (response == null)
            {
                logger.LogWarning("Appointment {AppointmentId} not found for deletion by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Appointment {AppointmentId} deleted successfully by user {UserId}", id, userId);
            return Ok(response);
        }


    }
}
