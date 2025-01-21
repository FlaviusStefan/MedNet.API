using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService labTestService;

        public LabTestsController(ILabTestService labTestService)
        {
            this.labTestService = labTestService;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateLabTest(CreateLabTestRequestDto request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var response = await labTestService.CreateLabTestAsync(request);
        //        return CreatedAtAction(nameof(GetLabTestById), new { id = response.Id }, response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An error occurred while creating the lab test.");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllLabTests()
        {
            var response = await labTestService.GetAllLabTestsAsync();
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetLabTestById(Guid id)
        {
            var response = await labTestService.GetLabTestByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateLabTesT(Guid id, UpdateLabTestRequestDto request)
        {
            var response = await labTestService.UpdateLabTestAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteLabTest(Guid id)
        {
            var response = await labTestService.DeleteLabTestAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
