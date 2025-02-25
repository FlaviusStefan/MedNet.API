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

        [Authorize(Roles = "Admin,Patient")]
        [HttpPost]
        public async Task<IActionResult> CreateInsurance(CreateInsuranceRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Extract role and userId from JWT
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid token. User ID not found." });
                }

                if (userRole == "Patient")
                {
                    // For patients, look up the patient record by the Identity user id
                    var patientRecord = await patientService.GetPatientByUserIdAsync(userId);
                    if (patientRecord == null)
                    {
                        return NotFound("Patient profile not found for the logged-in user.");
                    }
                    // Overwrite the request.PatientId with the actual patient record Id
                    request.PatientId = patientRecord.Id;
                }
                else
                {
                    // For Admin/Doctor, require a valid PatientId in the request
                    if (request.PatientId == null || request.PatientId == Guid.Empty)
                    {
                        return BadRequest("patientId is required for Admin or Doctor.");
                    }
                }

                var insuranceDto = await insuranceService.CreateInsuranceAsync(request);
                return CreatedAtAction(nameof(GetInsuranceById), new { id = insuranceDto.Id }, insuranceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the insurance: " + ex.Message);
            }
        }


        [Authorize(Roles = "Admin,Patient")] 
        [HttpGet]
        public async Task<IActionResult> GetAllInsurances()
        {
            var insurances = await insuranceService.GetAllInsurancesAsync();
            return Ok(insurances);
        }

        [Authorize(Roles = "Admin,Patient")] 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInsuranceById(Guid id)
        {
            var insuranceDto = await insuranceService.GetInsuranceByIdAsync(id);

            if (insuranceDto == null)
            {
                return NotFound();
            }

            return Ok(insuranceDto);
        }

        [Authorize(Roles = "Admin,Doctor")]
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
