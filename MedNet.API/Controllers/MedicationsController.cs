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

        public MedicationsController(IMedicationService medicationService, IPatientService patientService)
        {
            this.medicationService = medicationService;
            this.patientService = patientService;
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPost]
        public async Task<IActionResult> CreateMedication(CreateMedicationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var medicationDto = await medicationService.CreateMedicationAsync(request);
                return CreatedAtAction(nameof(GetMedicationById), new { id = medicationDto.Id }, medicationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the medication.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllMedications()
        {
            var medications = await medicationService.GetAllMedicationsAsync();
            return Ok(medications);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicationById(Guid id)
        {
            var medicationDto = await medicationService.GetMedicationByIdAsync(id);

            if(medicationDto == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != medicationDto.PatientId)
                {
                    return Forbid("You are not allowed to access this medication.");
                }
            }

            return Ok(medicationDto);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetMedicationsByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    return Forbid("You are not allowed to access other patients' medications.");
                }
            }

            var medications = await medicationService.GetMedicationsByPatientIdAsync(patientId);
            return Ok(medications);
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedication(Guid id, UpdateMedicationRequestDto request)
        {
            var response = await medicationService.UpdateMedicationAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteMedication(Guid id)
        {
            var response = await medicationService.DeleteMedicationAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
