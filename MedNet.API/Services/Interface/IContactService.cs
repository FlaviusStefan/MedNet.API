﻿using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IContactService
    {
        Task<ContactDto> CreateContactAsync(CreateContactRequestDto request);
        Task<IEnumerable<ContactDto>> GetAllContactsAsync();
        Task<ContactDto?> GetContactByIdAsync(Guid id);
    }
}