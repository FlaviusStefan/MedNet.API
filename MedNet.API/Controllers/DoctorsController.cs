using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
                var response = await _doctorService.CreateDoctorAsync(request);
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
            var response = await _doctorService.GetAllDoctorsAsync();
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            var response = await _doctorService.GetDoctorByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, UpdateDoctorRequestDto request)
        {
            var response = await _doctorService.UpdateDoctorAsync(id, request);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var response = await _doctorService.DeleteDoctorAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}