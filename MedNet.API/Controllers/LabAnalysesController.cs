using MedNet.API.Models.DTO;
using MedNet.API.Services;
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
    public class LabAnalysesController : ControllerBase
    {
        private readonly ILabAnalysisService labAnalysisService;
        private readonly IPatientService patientService;
        private readonly ILogger<LabAnalysesController> logger;

        public LabAnalysesController(ILabAnalysisService labAnalysisService, IPatientService patientService, ILogger<LabAnalysesController> logger)
        {
            this.labAnalysisService = labAnalysisService;
            this.patientService = patientService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateLabAnalysis(CreateLabAnalysisRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} creating lab analysis for Patient {PatientId}",
                userId, userRole, request.PatientId);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid lab analysis creation request from user {UserId}", userId);
                return BadRequest(ModelState);
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                logger.LogWarning("Lab analysis creation failed - Patient {PatientId} does not exist", request.PatientId);
                return BadRequest(new { error = $"Patient with ID {request.PatientId} does not exist." });
            }

            try
            {
                var response = await labAnalysisService.CreateLabAnalysisAsync(request);

                logger.LogInformation("Lab analysis {AnalysisId} created successfully by user {UserId} for Patient {PatientId} with {TestCount} tests",
                    response.Id, userId, request.PatientId, response.LabTests?.Count ?? 0);

                return CreatedAtAction(nameof(GetLabAnalysisById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating lab analysis for Patient {PatientId} by user {UserId}",
                    request.PatientId, userId);
                return StatusCode(500, new { error = "An error occurred while creating the lab analysis." });
            }
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllLabAnalyses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all lab analyses", userId, userRole);

            var response = await labAnalysisService.GetAllLabAnalysesAsync();

            logger.LogInformation("Returned {Count} lab analyses to user {UserId}",
                ((IEnumerable<DisplayLabAnalysisDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetLabAnalysisById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting lab analysis {AnalysisId}",
                userId, userRole, id);

            var response = await labAnalysisService.GetLabAnalysisByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Lab analysis {AnalysisId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != response.PatientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to lab analysis {AnalysisId} (belongs to Patient {PatientId})",
                        userId, id, response.PatientId);
                    return Forbid("You are not allowed to access this lab analysis.");
                }
            }

            logger.LogInformation("Lab analysis {AnalysisId} retrieved successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateLabAnalysis(Guid id, UpdateLabAnalysisRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} updating lab analysis {AnalysisId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid lab analysis update request from user {UserId} for analysis {AnalysisId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await labAnalysisService.UpdateLabAnalysisAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Lab analysis {AnalysisId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Lab analysis {AnalysisId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteLabAnalysis(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting lab analysis {AnalysisId}", userId, id);

            var response = await labAnalysisService.DeleteLabAnalysisAsync(id);

            if (response == null)
            {
                logger.LogWarning("Lab analysis {AnalysisId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Lab analysis {AnalysisId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}
