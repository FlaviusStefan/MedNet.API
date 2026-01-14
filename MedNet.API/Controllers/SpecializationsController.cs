using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService specializationService;
        private readonly ILogger<SpecializationsController> logger;

        public SpecializationsController(ISpecializationService specializationService, ILogger<SpecializationsController> logger)
        {
            this.specializationService = specializationService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSpecialization(CreateSpecializationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} creating specialization: {Name}", userId, request.Name);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid specialization creation request from admin {UserId}", userId);
                return BadRequest(ModelState);
            }

            try
            {
                var specializationDto = await specializationService.CreateSpecializationAsync(request);

                logger.LogInformation("Specialization {SpecializationId} created successfully by admin {UserId} - {Name}",
                    specializationDto.Id, userId, specializationDto.Name);

                return CreatedAtAction(nameof(GetSpecializationById), new { id = specializationDto.Id }, specializationDto);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Validation error creating specialization {Name} by admin {UserId}", request.Name, userId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating specialization {Name} by admin {UserId}",
                    request.Name, userId);
                return StatusCode(500, new { error = "An error occurred while creating the specialization." });
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllSpecializations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all specializations", userId, userRole);

            var specializations = await specializationService.GetAllSpecializationsAsync();

            logger.LogInformation("Returned {Count} specializations to user {UserId}",
                ((IEnumerable<SpecializationDto>)specializations).Count(), userId);

            return Ok(specializations);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecializationById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting specialization {SpecializationId}",
                userId, userRole, id);

            var specializationDto = await specializationService.GetSpecializationByIdAsync(id);

            if (specializationDto == null)
            {
                logger.LogWarning("Specialization {SpecializationId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Specialization {SpecializationId} retrieved successfully by user {UserId}", id, userId);
            return Ok(specializationDto);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetSpecializationsByDoctorId(Guid doctorId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting specializations for Doctor {DoctorId}",
                userId, userRole, doctorId);

            var specializations = await specializationService.GetSpecializationsByDoctorIdAsync(doctorId);

            logger.LogInformation("Returned {Count} specializations for Doctor {DoctorId} to user {UserId}",
                ((IEnumerable<SpecializationDto>)specializations).Count(), doctorId, userId);

            return Ok(specializations);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialization(Guid id, UpdateSpecializationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating specialization {SpecializationId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid specialization update request from admin {UserId} for specialization {SpecializationId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            try
            {
                var response = await specializationService.UpdateSpecializationAsync(id, request);
                if (response == null)
                {
                    logger.LogWarning("Specialization {SpecializationId} not found for update by admin {UserId}", id, userId);
                    return NotFound();
                }

                logger.LogInformation("Specialization {SpecializationId} updated successfully by admin {UserId}", id, userId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Validation error updating specialization {SpecializationId} by admin {UserId}", id, userId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteSpecialization(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting specialization {SpecializationId}", userId, id);

            var response = await specializationService.DeleteSpecializationAsync(id);

            if (response == null)
            {
                logger.LogWarning("Specialization {SpecializationId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Specialization {SpecializationId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}