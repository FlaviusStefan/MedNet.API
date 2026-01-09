using MedNet.API.Exceptions;
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
    public class MedicalFilesController : ControllerBase
    {
        private readonly IMedicalFileService medicalFileService;
        private readonly IPatientService patientService;
        private readonly ILogger<MedicalFilesController> logger;


        public MedicalFilesController(IMedicalFileService medicalFileService, IPatientService patientService, ILogger<MedicalFilesController> logger)
        {
            this.medicalFileService = medicalFileService;
            this.patientService = patientService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPost]
        public async Task<IActionResult> CreateMedicalFile(CreateMedicalFileRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} creating medical file for Patient {PatientId}",
                userId, userRole, request.PatientId);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid medical file creation request from user {UserId}", userId);
                return BadRequest(ModelState);
            }

            var patient = await patientService.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                logger.LogWarning("Medical file creation failed - Patient {PatientId} does not exist", request.PatientId);
                return BadRequest(new { error = $"Patient with ID {request.PatientId} does not exist." });
            }

            try
            {
                var medicalFileDto = await medicalFileService.CreateMedicalFileAsync(request);

                logger.LogInformation("Medical file {FileId} created successfully by user {UserId} for Patient {PatientId}",
                    medicalFileDto.Id, userId, request.PatientId);

                return CreatedAtAction(nameof(GetMedicalFileById), new { id = medicalFileDto.Id }, medicalFileDto);
            }
            catch (CustomException ex)
            {
                logger.LogWarning(ex, "Medical file creation failed for Patient {PatientId} by user {UserId}: {Message}",
                    request.PatientId, userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating medical file for Patient {PatientId} by user {UserId}",
                    request.PatientId, userId);
                return StatusCode(500, new { error = "An error occurred while creating the medical file." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalFiles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} requesting all medical files", userId);

            var medicalFiles = await medicalFileService.GetAllMedicalFilesAsync();

            logger.LogInformation("Returned {Count} medical files to admin {UserId}",
                ((IEnumerable<MedicalFileDto>)medicalFiles).Count(), userId);

            return Ok(medicalFiles);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMedicalFileById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting medical file {FileId}",
                userId, userRole, id);

            var medicalFileDto = await medicalFileService.GetMedicalFileByIdAsync(id);
            if (medicalFileDto == null)
            {
                logger.LogWarning("Medical file {FileId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != medicalFileDto.PatientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to medical file {FileId} (belongs to Patient {PatientId})",
                        userId, id, medicalFileDto.PatientId);
                    return Forbid("You are not allowed to access this medical file.");
                }
            }

            logger.LogInformation("Medical file {FileId} retrieved successfully by user {UserId}", id, userId);
            return Ok(medicalFileDto);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetMedicalFilesByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting medical files for Patient {PatientId}",
                userId, userRole, patientId);

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    logger.LogWarning("Patient {UserId} denied access to medical files for Patient {PatientId}",
                        userId, patientId);
                    return Forbid("You are not allowed to access other patients' medical files.");
                }
            }

            var medicalFiles = await medicalFileService.GetMedicalFilesByPatientIdAsync(patientId);

            logger.LogInformation("Returned {Count} medical files for Patient {PatientId} to user {UserId}",
                ((IEnumerable<MedicalFileDto>)medicalFiles).Count(), patientId, userId);

            return Ok(medicalFiles);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalFile(Guid id, UpdateMedicalFileRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} updating medical file {FileId}",
                userId, userRole, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid medical file update request from user {UserId} for file {FileId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await medicalFileService.UpdateMedicalFileAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Medical file {FileId} not found for update by user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Medical file {FileId} updated successfully by user {UserId}", id, userId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteMedicalFile(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} deleting medical file {FileId}", userId, id);

            var response = await medicalFileService.DeleteMedicalFileAsync(id);

            if (response == null)
            {
                logger.LogWarning("Medical file {FileId} not found for deletion by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Medical file {FileId} deleted successfully by admin {UserId}", id, userId);
            return Ok(new { message = response });
        }
    }
}