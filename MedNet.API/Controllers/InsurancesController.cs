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

        public InsurancesController(IInsuranceService insuranceService, IPatientService patientService)
        {
            this.insuranceService = insuranceService;
            this.patientService = patientService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateInsurance(CreateInsuranceRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.PatientId == null || request.PatientId == Guid.Empty)
            {
                return BadRequest("patientId is required.");
            }

            try
            {
                var insuranceDto = await insuranceService.CreateInsuranceAsync(request);
                return CreatedAtAction(nameof(GetInsuranceById), new { id = insuranceDto.Id }, insuranceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the insurance: " + ex.Message);
            }
        }


        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public async Task<IActionResult> GetAllInsurances()
        {
            var insurances = await insuranceService.GetAllInsurancesAsync();
            return Ok(insurances);
        }


        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInsuranceById(Guid id)
        {
            var insuranceDto = await insuranceService.GetInsuranceByIdAsync(id);
            if (insuranceDto == null)
            {
                return NotFound();
            }

            // Additional check for patients: ensure they can only access their own insurance.
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != insuranceDto.PatientId)
                {
                    return Forbid("You are not allowed to access this insurance.");
                }
            }

            return Ok(insuranceDto);
        }


        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetInsurancesByPatientId(Guid patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userRole == "Patient")
            {
                var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                if (patientRecord == null || patientRecord.Id != patientId)
                {
                    return Forbid("You are not allowed to access other patients' insurances.");
                }
            }

            var insurances = await insuranceService.GetInsurancesByPatientIdAsync(patientId);
            return Ok(insurances);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInsurance(Guid id, UpdateInsuranceRequestDto request)
        {
            var response = await insuranceService.UpdateInsuranceAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteInsurance(Guid id)
        {
            var response = await insuranceService.DeleteInsuranceAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
