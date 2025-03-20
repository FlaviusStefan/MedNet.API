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

        public MedicalFilesController(IMedicalFileService medicalFileService, IPatientService patientService)
        {
            this.medicalFileService = medicalFileService;
            this.patientService = patientService;
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPost]
        public async Task<IActionResult> CreateMedicalFile(CreateMedicalFileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var medicalFileDto = await medicalFileService.CreateMedicalFileAsync(request);
                return CreatedAtAction(nameof(GetMedicalFileById), new { id = medicalFileDto.Id }, medicalFileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the medical file.");
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalFiles()
        {
            var medicalFiles = await medicalFileService.GetAllMedicalFilesAsync();
            return Ok(medicalFiles);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMedicalFileById(Guid id)
        {
            var medicalFileDto = await medicalFileService.GetMedicalFileByIdAsync(id);
            if (medicalFileDto == null)
            {
                return NotFound();
            }

            // Additional check for patients: ensure they can only access their own medical files.
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != medicalFileDto.PatientId)
                {
                    return Forbid("You are not allowed to access this medical file.");
                }
            }

            return Ok(medicalFileDto);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetMedicalFilesByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    return Forbid("You are not allowed to access other patients' medical files.");
                }
            }

            var medicalFiles = await medicalFileService.GetMedicalFilesByPatientIdAsync(patientId);
            return Ok(medicalFiles);
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalFile(Guid id, UpdateMedicalFileRequestDto request)
        {
            var response = await medicalFileService.UpdateMedicalFileAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")] 
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteMedicalFile(Guid id)
        {
            var response = await medicalFileService.DeleteMedicalFileAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }


    }
}
