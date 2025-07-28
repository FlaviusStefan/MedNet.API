using MedNet.API.Models.DTO;
using MedNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            this.doctorService = doctorService;
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public async Task<IActionResult> CreateDoctor(CreateDoctorRequestDto request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var doctor = await doctorService.CreateDoctorAsync(request);

                
        //        var response = new
        //        {
        //            message = "Doctor created successfully.",
        //            doctor
        //        };

        //        return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, new {  response });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An error occurred while creating the doctor.");
        //    }
        //}


        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var response = await doctorService.GetAllDoctorsAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, UpdateDoctorRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDoctor = await doctorService.UpdateDoctorAsync(id, request);

                if (updatedDoctor == null)
                {
                    return NotFound("Doctor not found!");
                }

                // Return the message and the updated doctor object
                var response = new
                {
                    message = "Doctor updated successfully!",
                    doctor = updatedDoctor
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the doctor!");
            }
        }

        [Authorize(Roles = "Admin")]
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