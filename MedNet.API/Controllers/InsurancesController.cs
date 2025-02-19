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
    public class InsurancesController : ControllerBase
    {
        private readonly IInsuranceService insuranceService;

        public InsurancesController(IInsuranceService insuranceService)
        {
            this.insuranceService = insuranceService;
        }

        [Authorize(Roles = "Admin,Doctor")]

        [HttpPost]
        public async Task<IActionResult> CreateInsurance(CreateInsuranceRequestDto request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var insuranceDto = await insuranceService.CreateInsuranceAsync(request);
                return CreatedAtAction(nameof(GetInsuranceById), new { id = insuranceDto.Id }, insuranceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the insurance.");
            }
        }


        [Authorize(Roles = "Admin,Doctor,Patient")] 
        [HttpGet]
        public async Task<IActionResult> GetAllInsurances()
        {
            var insurances = await insuranceService.GetAllInsurancesAsync();
            return Ok(insurances);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")] 
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
