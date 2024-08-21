using MedNet.API.Models.DTO;
using MedNet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            this.doctorService = doctorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(CreateDoctorRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await doctorService.CreateDoctorAsync(request);
                return CreatedAtAction(nameof(GetDoctorById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the doctor.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var response = await doctorService.GetAllDoctorsAsync();
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            var response = await doctorService.GetDoctorByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, UpdateDoctorRequestDto request)
        {
            var response = await doctorService.UpdateDoctorAsync(id, request);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var response = await doctorService.DeleteDoctorAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}