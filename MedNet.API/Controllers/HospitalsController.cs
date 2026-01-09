using MedNet.API.Exceptions;
using MedNet.API.Models.DTO;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly IHospitalService hospitalService;
        private readonly ILogger<HospitalsController> logger;

        public HospitalsController(IHospitalService hospitalService, ILogger<HospitalsController> logger)
        {
            this.hospitalService = hospitalService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all hospitals",
                userId, userRole);

            var response = await hospitalService.GetAllHospitalsAsync();

            logger.LogInformation("Returned {Count} hospitals to user {UserId}",
                ((IEnumerable<HospitalResponseDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetHospitalById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting hospital {HospitalId}",
                userId, userRole, id);

            var response = await hospitalService.GetHospitalByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Hospital {HospitalId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateHospital(Guid id, UpdateHospitalRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating hospital {HospitalId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid hospital update request from admin {UserId} for hospital {HospitalId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            try
            {
                var response = await hospitalService.UpdateHospitalAsync(id, request);

                if (response == null)
                {
                    logger.LogWarning("Hospital {HospitalId} not found for update by admin {UserId}",
                        id, userId);
                    return NotFound();
                }

                logger.LogInformation("Hospital {HospitalId} updated successfully by admin {UserId}",
                    id, userId);
                return Ok(new { message = "Hospital updated successfully!", hospital = response });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while admin {UserId} updating hospital {HospitalId}",
                    userId, id);
                return StatusCode(500, new { error = "An error occurred while updating the hospital." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteHospital(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting hospital {HospitalId}", userId, id);

            try
            {
                var response = await hospitalService.DeleteHospitalAsync(id);

                if (response == null)
                {
                    logger.LogWarning("Hospital {HospitalId} not found for deletion by admin {UserId}",
                        id, userId);
                    return NotFound();
                }

                logger.LogInformation("Hospital {HospitalId} deleted successfully by admin {UserId}",
                    id, userId);
                return Ok(new { message = response });
            }
            catch (CustomException ex)
            {
                logger.LogError(ex, "Failed to delete hospital {HospitalId} by admin {UserId}: {Message}",
                    id, userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while admin {UserId} deleting hospital {HospitalId}",
                    userId, id);
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the hospital." });
            }
        }
    }
}