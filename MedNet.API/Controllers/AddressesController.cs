using MedNet.API.Models.DTO;
using MedNet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService addressService;

        public AddressesController(IAddressService addressService)
        {
            this.addressService = addressService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var addressDto = await addressService.CreateAddressAsync(request);
                return CreatedAtAction(nameof(GetAddressById), new { id = addressDto.Id }, addressDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the doctor.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await addressService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var addressDto = await addressService.GetAddressByIdAsync(id);

            if (addressDto == null)
            {
                return NotFound();
            }

            return Ok(addressDto);
        }
    }
}
