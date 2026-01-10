using MedNet.API.Models.DTO;
using MedNet.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService addressService;
        private readonly ILogger<AddressesController> logger;

        public AddressesController(IAddressService addressService, ILogger<AddressesController> logger)
        {
            this.addressService = addressService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all addresses", userId, userRole);

            var addresses = await addressService.GetAllAddressesAsync();

            logger.LogInformation("Returned {Count} addresses to user {UserId}",
                ((IEnumerable<ContactDto>)addresses).Count(), userId);
            return Ok(addresses);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting address {AddressId}",
                userId, userRole, id);

            var addressDto = await addressService.GetAddressByIdAsync(id);

            if (addressDto == null)
            {
                logger.LogWarning("Address {AddressId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(addressDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id,UpdateAddressRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating address {AddressId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid address update request from admin {UserId} for address {AddressId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await addressService.UpdateAddressAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Address {AddressId} not found for update by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Address {ContactId} updated successfully by admin {UserId}", id, userId);
            return Ok(response);
        }
    }
}
