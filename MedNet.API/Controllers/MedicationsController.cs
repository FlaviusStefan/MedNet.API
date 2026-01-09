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
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService medicationService;
        private readonly IPatientService patientService;
        private readonly ILogger<MedicationsController> logger;

        public MedicationsController(IMedicationService medicationService, IPatientService patientService, ILogger<MedicationsController> logger)
        {
            this.medicationService = medicationService;
            this.patientService = patientService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreateMedication(CreateMedicationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} creating medication for Patient {PatientId}",
                userId, userRole, request.PatientId);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid medication creation request from user {UserId}", userId);
                return BadRequest(ModelState);
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                logger.LogWarning("Medication creation failed - Patient {PatientId} does not exist", request.PatientId);
                return BadRequest(new { error = $"Patient with ID {request.PatientId} does not exist." });
            }

            try
            {
                var medicationDto = await medicationService.CreateMedicationAsync(request);

                logger.LogInformation("Medication {MedicationId} created successfully by user {UserId} for Patient {PatientId}",
                    medicationDto.Id, userId, request.PatientId);

                return CreatedAtAction(nameof(GetMedicationById), new { id = medicationDto.Id }, medicationDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating medication for Patient {PatientId} by user {UserId}",
                    request.PatientId, userId);
                return StatusCode(500, new { error = "An error occurred while creating the medication." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllMedications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} requesting all medications", userId);

            var medications = await medicationService.GetAllMedicationsAsync();

            logger.LogInformation("Returned {Count} medications to admin {UserId}",
                ((IEnumerable<MedicationDto>)medications).Count(), userId);

            return Ok(medications);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicationById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting medication {MedicationId}",
                userId, userRole, id);

            var medicationDto = await medicationService.GetMedicationByIdAsync(id);

            if (medicationDto == null)
            {
                logger.LogWarning("Medication {MedicationId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != medicationDto.PatientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to medication {MedicationId} (belongs to Patient {PatientId})",
                        userId, id, medicationDto.PatientId);
                    return Forbid("You are not allowed to access this medication.");
                }
            }

            logger.LogInformation("Medication {MedicationId} retrieved successfully by user {UserId}", id, userId);
            return Ok(medicationDto);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetMedicationsByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting medications for Patient {PatientId}",
                userId, userRole, patientId);

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to medications for Patient {PatientId}",
                        userId, patientId);
                    return Forbid("You are not allowed to access other patients' medications.");
                }
            }

            var medications = await medicationService.GetMedicationsByPatientIdAsync(patientId);

            logger.LogInformation("Returned {Count} medications for Patient {PatientId} to user {UserId}",
                ((IEnumerable<MedicationDto>)medications).Count(), patientId, userId);

            return Ok(medications);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedication(Guid id, UpdateMedicationRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} updating medication {MedicationId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid medication update request from user {UserId} for medication {MedicationId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await medicationService.UpdateMedicationAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Medication {MedicationId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Medication {MedicationId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteMedication(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting medication {MedicationId}", userId, id);

            var response = await medicationService.DeleteMedicationAsync(id);

            if (response == null)
            {
                logger.LogWarning("Medication {MedicationId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Medication {MedicationId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}