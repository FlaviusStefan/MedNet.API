using MedNet.API.Exceptions;
using MedNet.API.Models.DTO;
using MedNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService doctorService;
        private readonly ILogger<DoctorsController> logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            this.doctorService = doctorService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all doctors", userId, userRole);

            var response = await doctorService.GetAllDoctorsAsync();

            logger.LogInformation("Returned {Count} doctors to user {UserId}",
                ((IEnumerable<DoctorResponseDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting doctor {DoctorId}",
                userId, userRole, id);

            var response = await doctorService.GetDoctorByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Doctor {DoctorId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, UpdateDoctorRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating doctor {DoctorId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid doctor update request from admin {UserId} for doctor {DoctorId}: {ValidationErrors}",
                    userId, id, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDoctor = await doctorService.UpdateDoctorAsync(id, request);

                if (updatedDoctor == null)
                {
                    logger.LogWarning("Doctor {DoctorId} not found for update by admin {UserId}", id, userId);
                    return NotFound(new { error = "Doctor not found!" });
                }

                logger.LogInformation("Doctor {DoctorId} updated successfully by admin {UserId}", id, userId);

                // Return the message and the updated doctor object
                var response = new
                {
                    message = "Doctor updated successfully!",
                    doctor = updatedDoctor
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning("Doctor update validation failed for admin {UserId}: {Message}", userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while admin {UserId} updating doctor {DoctorId}", userId, id);
                return StatusCode(500, new { error = "An error occurred while updating the doctor!" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting doctor {DoctorId}", userId, id);

            try
            {
                var response = await doctorService.DeleteDoctorAsync(id);

                if (response == null)
                {
                    logger.LogWarning("Doctor {DoctorId} not found for deletion by admin {UserId}", id, userId);
                    return NotFound();
                }

                logger.LogInformation("Doctor {DoctorId} deleted successfully by admin {UserId}", id, userId);
                return Ok(new { message = response });
            }
            catch (CustomException ex)
            {
                logger.LogError(ex, "Failed to delete doctor {DoctorId} by admin {UserId}: {Message}", id, userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while admin {UserId} deleting doctor {DoctorId}", userId, id);
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the doctor." });
            }
        }
    }
}