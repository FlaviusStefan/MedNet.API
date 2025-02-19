using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalFilesController : ControllerBase
    {
        private readonly IMedicalFileService medicalFileService;

        public MedicalFilesController(IMedicalFileService medicalFileService)
        {
            this.medicalFileService = medicalFileService;
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

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalFileById(Guid id)
        {
            var medicalFileDto = await medicalFileService.GetMedicalFileByIdAsync(id);

            if (medicalFileDto == null)
            {
                return NotFound();
            }

            return Ok(medicalFileDto);
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
