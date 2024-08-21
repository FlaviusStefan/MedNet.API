﻿using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            this.contactRepository = contactRepository;
        }
        public async Task<ContactDto> CreateContactAsync(CreateContactRequestDto request)
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Phone = request.Phone,
                Email = request.Email
            };

            await contactRepository.CreateAsync(contact);

            return new ContactDto
            {
                Id = contact.Id,
                Phone = contact.Phone,
                Email = contact.Email
            };
        }

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
        {
            var contacts = await contactRepository.GetAllAsync();

            return contacts.Select(contact => new ContactDto
            {
                Id = contact.Id,
                Phone = contact.Phone,
                Email = contact.Email
            }).ToList();
        }

        public async Task<ContactDto?> GetContactByIdAsync(Guid id)
        {
            var contact = await contactRepository.GetById(id);
            if(contact == null)
            {
                return null;
            }

            return new ContactDto
            {
                Id = contact.Id,
                Phone = contact.Phone,
                Email = contact.Email
            };
        }
    }
}