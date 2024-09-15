using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var response = await patientService.GetAllPatientsAsync();
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var response = await patientService.GetPatientByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdatePatient(Guid id, UpdatePatientRequestDto request)
        {
            var response = await patientService.UpdatePatientAsync(id, request);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }


    }
}
