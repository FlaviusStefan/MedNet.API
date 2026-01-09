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
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService labTestService;
        private readonly ILabAnalysisService labAnalysisService;
        private readonly IPatientService patientService;
        private readonly ILogger<LabTestsController> logger;

        public LabTestsController(
            ILabTestService labTestService,
            ILabAnalysisService labAnalysisService,
            IPatientService patientService,
            ILogger<LabTestsController> logger)
        {
            this.labTestService = labTestService;
            this.labAnalysisService = labAnalysisService;
            this.patientService = patientService;
            this.logger = logger;
        }

        // ❌ KEPT COMMENTED - Lab tests are created as part of lab analysis creation
        // Lab tests should be created via LabAnalysisService.CreateLabAnalysisAsync
        // This ensures proper transactional integrity
        //[Authorize(Roles = "Admin,Doctor")]
        //[HttpPost]
        //public async Task<IActionResult> CreateLabTest(CreateLabTestRequestDto request) { }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllLabTests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all lab tests", userId, userRole);

            var response = await labTestService.GetAllLabTestsAsync();

            logger.LogInformation("Returned {Count} lab tests to user {UserId}",
                ((IEnumerable<LabTestDto>)response).Count(), userId);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetLabTestById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting lab test {TestId}",
                userId, userRole, id);

            var response = await labTestService.GetLabTestByIdAsync(id);

            if (response == null)
            {
                logger.LogWarning("Lab test {TestId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null)
                {
                    logger.LogWarning("Patient record not found for user {UserId}", userId);
                    return Forbid("You are not allowed to access this lab test.");
                }

                // Get the lab analysis that owns this test and check its PatientId
                var analysis = await labAnalysisService.GetLabAnalysisByIdAsync(response.LabAnalysisId);
                if (analysis == null || analysis.PatientId != patientRecord.Id)
                {
                    logger.LogWarning("Patient {UserId} denied access to lab test {TestId} (belongs to LabAnalysis {AnalysisId} for Patient {PatientId})",
                        userId, id, response.LabAnalysisId, analysis?.PatientId);
                    return Forbid("You are not allowed to access this lab test.");
                }
            }

            logger.LogInformation("Lab test {TestId} retrieved successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateLabTest(Guid id, UpdateLabTestRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} updating lab test {TestId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid lab test update request from user {UserId} for test {TestId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await labTestService.UpdateLabTestAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Lab test {TestId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Lab test {TestId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteLabTest(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting lab test {TestId}", userId, id);

            var response = await labTestService.DeleteLabTestAsync(id);

            if (response == null)
            {
                logger.LogWarning("Lab test {TestId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Lab test {TestId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}