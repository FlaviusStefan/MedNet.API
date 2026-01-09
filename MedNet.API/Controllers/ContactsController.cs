using MedNet.API.Models.DTO;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService contactService;
        private readonly ILogger<ContactsController> logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            this.contactService = contactService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting all contacts", userId, userRole);

            var contacts = await contactService.GetAllContactsAsync();

            logger.LogInformation("Returned {Count} contacts to user {UserId}",
                ((IEnumerable<ContactDto>)contacts).Count(), userId);

            return Ok(contacts);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting contact {ContactId}",
                userId, userRole, id);

            var contactDto = await contactService.GetContactByIdAsync(id);

            if (contactDto == null)
            {
                logger.LogWarning("Contact {ContactId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(contactDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(Guid id, UpdateContactRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} updating contact {ContactId}", userId, id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid contact update request from admin {UserId} for contact {ContactId}",
                    userId, id);
                return BadRequest(ModelState);
            }

            var response = await contactService.UpdateContactAsync(id, request);
            if (response == null)
            {
                logger.LogWarning("Contact {ContactId} not found for update by admin {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("Contact {ContactId} updated successfully by admin {UserId}", id, userId);
            return Ok(response);
        }
    }
}
