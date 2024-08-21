﻿using MedNet.API.Models.DTO;
using MedNet.API.Services;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService contactService;

        public ContactsController(IContactService contactService)
        {
            this.contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(CreateContactRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var contactDto = await contactService.CreateContactAsync(request);
                return CreatedAtAction(nameof(GetContactById), new { id = contactDto.Id }, contactDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the contact.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await contactService.GetAllContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(Guid id)
        {
            var contactDto = await contactService.GetContactByIdAsync(id);

            if (contactDto == null)
            {
                return NotFound();
            }

            return Ok(contactDto);
        }
    }
}