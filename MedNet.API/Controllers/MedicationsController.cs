using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            this.medicationService = medicationService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllMedications()
        {
            var medications = await medicationService.GetAllMedicationsAsync();
            return Ok(medications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicationById(Guid id)
        {
            var medicationDto = await medicationService.GetMedicationByIdAsync(id);

            if(medicationDto == null)
            {
                return NotFound();
            }

            return Ok(medicationDto);
        }

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
