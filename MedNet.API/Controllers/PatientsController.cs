using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService patientService;

        public PatientsController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [Authorize(Roles = "Admin")] 
        [HttpPost]
        public async Task<IActionResult> CreatePatient(CreatePatientRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await patientService.CreatePatientAsync(request);
                return CreatedAtAction(nameof(GetPatientById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the patient.");
            }
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var response = await patientService.GetAllPatientsAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            // Extract the correct UserId from JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            Console.WriteLine("=========================================");
            Console.WriteLine($"[DEBUG] Extracted UserId: '{userId}'");
            Console.WriteLine("=========================================");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[ERROR] UserId not found in token.");
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }

            // Fetch patient using UserId
            var patient = await patientService.GetPatientByUserIdAsync(userId);

            if (patient == null)
            {
                Console.WriteLine($"[ERROR] No patient found for UserId: {userId}");
                return NotFound(new { message = "Patient profile not found." });
            }

            return Ok(patient);
        }



        // ✅ Doctors & Admins can get any patient, but Patients can only get their own
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Patient")
            {
                var patient = await patientService.GetPatientByUserIdAsync(userId);
                if (patient == null || patient.Id != id)
                {
                    return Forbid("You are not allowed to access this patient data.");
                }
            }

            var response = await patientService.GetPatientByIdAsync(id);
            return response != null ? Ok(response) : NotFound();
        }

        // ✅ Doctors & Admins can update any patient, but Patients can only update their own profile
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdatePatient(Guid id, UpdatePatientRequestDto request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Patient")
            {
                var patient = await patientService.GetPatientByUserIdAsync(userId);
                if (patient == null || patient.Id != id)
                {
                    return Forbid("You can only update your own profile.");
                }
            }

            var response = await patientService.UpdatePatientAsync(id, request);
            return response != null ? Ok(response) : NotFound();
        }

        // ✅ Only Admins can delete patients
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var response = await patientService.DeletePatientAsync(id);
            return response != null ? Ok(response) : NotFound();
        }

    }
}
