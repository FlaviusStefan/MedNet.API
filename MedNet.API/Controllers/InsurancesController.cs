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
    public class InsurancesController : ControllerBase
    {
        private readonly IInsuranceService insuranceService;
        private readonly IPatientService patientService;
        private readonly ILogger<InsurancesController> logger;

        public InsurancesController(
            IInsuranceService insuranceService,
            IPatientService patientService,
            ILogger<InsurancesController> logger)
        {
            this.insuranceService = insuranceService;
            this.patientService = patientService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateInsurance(CreateInsuranceRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} creating insurance for Patient {PatientId}",
                userId, request.PatientId);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid insurance creation request from admin {UserId}", userId);
                return BadRequest(ModelState);
            }

            if (request.PatientId == null || request.PatientId == Guid.Empty)
            {
                logger.LogWarning("Insurance creation failed - PatientId is missing for admin {UserId}", userId);
                return BadRequest("patientId is required.");
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId.Value);
            if (patient == null)
            {
                logger.LogWarning("Insurance creation failed - Patient {PatientId} does not exist", request.PatientId);
                return BadRequest(new { error = $"Patient with ID {request.PatientId.Value} does not exist." });
            }

            try
            {
                var insuranceDto = await insuranceService.CreateInsuranceAsync(request);

                logger.LogInformation("Insurance {InsuranceId} created successfully by admin {UserId} for Patient {PatientId}",
                    insuranceDto.Id, userId, request.PatientId);

                return CreatedAtAction(nameof(GetInsuranceById), new { id = insuranceDto.Id }, insuranceDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating insurance for Patient {PatientId} by admin {UserId}",
                    request.PatientId, userId);
                return StatusCode(500, "An error occurred while creating the insurance: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllInsurances()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} requesting all insurances", userId);

            var insurances = await insuranceService.GetAllInsurancesAsync();

            logger.LogInformation("Returned {Count} insurances to admin {UserId}",
                ((IEnumerable<InsuranceDto>)insurances).Count(), userId);

            return Ok(insurances);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInsuranceById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting insurance {InsuranceId}",
                userId, userRole, id);

            var response = await insuranceService.GetInsuranceByIdAsync(id);
            if (response == null)
            {
                logger.LogWarning("Insurance {InsuranceId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != response.PatientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to insurance {InsuranceId} (belongs to Patient {PatientId})",
                        userId, id, response.PatientId);
                    return Forbid("You are not allowed to access this insurance.");
                }
            }

            logger.LogInformation("Insurance {InsuranceId} retrieved successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetInsurancesByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting insurances for Patient {PatientId}",
                userId, userRole, patientId);

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to insurances for Patient {PatientId}",
                        userId, patientId);
                    return Forbid("You are not allowed to access other patients' insurances.");
                }
            }

            var insurances = await insuranceService.GetInsurancesByPatientIdAsync(patientId);

            logger.LogInformation("Returned {Count} insurances for Patient {PatientId} to user {UserId}",
                ((IEnumerable<InsuranceDto>)insurances).Count(), patientId, userId);

            return Ok(insurances);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInsurance(Guid id, UpdateInsuranceRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating insurance {InsuranceId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid insurance update request from admin {UserId} for insurance {InsuranceId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await insuranceService.UpdateInsuranceAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Insurance {InsuranceId} not found for update by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Insurance {InsuranceId} updated successfully by admin {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteInsurance(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting insurance {InsuranceId}", userId, id);

            var response = await insuranceService.DeleteInsuranceAsync(id);

            if (response == null)
            {
                logger.LogWarning("Insurance {InsuranceId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Insurance {InsuranceId} deleted successfully by admin {UserId}", id, userId);
            return Ok(response);
        }
    }
}