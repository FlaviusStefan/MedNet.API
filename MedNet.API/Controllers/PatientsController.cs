using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService patientService;
        private readonly ILogger<PatientsController> logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            this.patientService = patientService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all patients", userId, userRole);

            var response = await patientService.GetAllPatientsAsync();

            logger.LogInformation("Returned {Count} patients to user {UserId}",
                ((IEnumerable<PatientBasicSummaryDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            logger.LogInformation("Patient {UserId} requesting their own profile", userId);

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("Patient profile request failed - UserId not found in token");
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }

            var patient = await patientService.GetPatientByUserIdAsync(userId);

            if (patient == null)
            {
                logger.LogWarning("Patient profile not found for UserId: {UserId}", userId);
                return NotFound(new { message = "Patient profile not found." });
            }

            logger.LogInformation("Successfully retrieved patient profile for UserId: {UserId}", userId);
            return Ok(patient);
        }



        // ✅ Doctors & Admins can get any patient, but Patients can only get their own
        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting patient {PatientId}",
                userId, userRole, id);

            var response = await patientService.GetPatientByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Patient {PatientId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Successfully retrieved patient {PatientId} for user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdatePatient(Guid id, UpdatePatientRequestDto request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("User {UserId} with role {Role} attempting to update patient {PatientId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid patient update request from user {UserId} for patient {PatientId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            if (userRole == "Patient")
            {
                var patient = await patientService.GetPatientByUserIdAsync(userId);
                if (patient == null || patient.Id != id)
                {
                    logger.LogWarning("Patient {UserId} denied permission to update patient {PatientId}",
                        userId, id);
                    return Forbid("You can only update your own profile.");
                }
            }

            var response = await patientService.UpdatePatientAsync(id, request);

            if (response == null)
            {
                logger.LogWarning("Patient {PatientId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Patient {PatientId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} attempting to delete patient {PatientId}", userId, id);

            var response = await patientService.DeletePatientAsync(id);

            if (response == null)
            {
                logger.LogWarning("Patient {PatientId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Patient {PatientId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }

    }
}
