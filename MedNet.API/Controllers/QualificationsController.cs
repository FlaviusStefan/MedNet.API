using MedNet.API.Models.DTO;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QualificationsController : ControllerBase
    {
        private readonly IQualificationService qualificationService;
        private readonly IDoctorService doctorService;
        private readonly ILogger<QualificationsController> logger;

        public QualificationsController(
            IQualificationService qualificationService,
            IDoctorService doctorService,
            ILogger<QualificationsController> logger)
        {
            this.qualificationService = qualificationService;
            this.doctorService = doctorService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQualification(CreateQualificationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} creating qualification for Doctor {DoctorId}: {Degree} from {Institution}",
                userId, request.DoctorId, request.Degree, request.Institution);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid qualification creation request from admin {UserId}", userId);
                return BadRequest(ModelState);
            }

            var doctor = await doctorService.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                logger.LogWarning("Qualification creation failed - Doctor {DoctorId} does not exist", request.DoctorId);
                return BadRequest(new { error = $"Doctor with ID {request.DoctorId} does not exist." });
            }

            try
            {
                var qualificationDto = await qualificationService.CreateQualificationAsync(request);

                logger.LogInformation("Qualification {QualificationId} created successfully by admin {UserId} for Doctor {DoctorId} - {Degree} from {Institution}",
                    qualificationDto.Id, userId, request.DoctorId, qualificationDto.Degree, qualificationDto.Institution);

                return CreatedAtAction(nameof(GetQualificationById), new { id = qualificationDto.Id }, qualificationDto);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Validation error creating qualification for Doctor {DoctorId} by admin {UserId}",
                    request.DoctorId, userId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating qualification for Doctor {DoctorId} by admin {UserId}",
                    request.DoctorId, userId);
                return StatusCode(500, new { error = "An error occurred while creating the qualification." });
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllQualifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all qualifications", userId, userRole);

            var qualifications = await qualificationService.GetAllQualificationsAsync();

            logger.LogInformation("Returned {Count} qualifications to user {UserId}",
                ((IEnumerable<QualificationDto>)qualifications).Count(), userId);

            return Ok(qualifications);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQualificationById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting qualification {QualificationId}",
                userId, userRole, id);

            var qualificationDto = await qualificationService.GetQualificationByIdAsync(id);

            if (qualificationDto == null)
            {
                logger.LogWarning("Qualification {QualificationId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Qualification {QualificationId} retrieved successfully by user {UserId}", id, userId);
            return Ok(qualificationDto);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetQualificationsByDoctorId(Guid doctorId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting qualifications for Doctor {DoctorId}",
                userId, userRole, doctorId);

            var qualifications = await qualificationService.GetQualificationsByDoctorIdAsync(doctorId);

            logger.LogInformation("Returned {Count} qualifications for Doctor {DoctorId} to user {UserId}",
                ((IEnumerable<QualificationDto>)qualifications).Count(), doctorId, userId);

            return Ok(qualifications);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualification(Guid id, UpdateQualificationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating qualification {QualificationId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid qualification update request from admin {UserId} for qualification {QualificationId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            try
            {
                var response = await qualificationService.UpdateQualificationAsync(id, request);
                if (response == null)
                {
                    logger.LogWarning("Qualification {QualificationId} not found for update by admin {UserId}", id, userId);
                    return NotFound();
                }

                logger.LogInformation("Qualification {QualificationId} updated successfully by admin {UserId}", id, userId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Validation error updating qualification {QualificationId} by admin {UserId}", id, userId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteQualification(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting qualification {QualificationId}", userId, id);

            var response = await qualificationService.DeleteQualificationAsync(id);

            if (response == null)
            {
                logger.LogWarning("Qualification {QualificationId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Qualification {QualificationId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}