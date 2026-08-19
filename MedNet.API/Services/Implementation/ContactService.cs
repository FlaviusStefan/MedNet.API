using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository contactRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ContactService> logger;

        public ContactService(IContactRepository contactRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContactService> logger)
        {
            this.contactRepository = contactRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<ContactDto> CreateContactAsync(CreateContactRequestDto request)
        {
            logger.LogInformation("Creating new contact with Email: {Email}, Phone: {Phone}", request.Email, request.Phone);

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Phone = request.Phone,
                Email = request.Email
            };

            await contactRepository.CreateAsync(contact);

            logger.LogInformation("Contact {ContactId} created (pending transaction commit)", contact.Id);

            return mapper.Map<ContactDto>(contact);
        }

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
        {
            logger.LogInformation("Retrieving all contacts");

            var contacts = await contactRepository.GetAllAsync();
            var contactList = mapper.Map<List<ContactDto>>(contacts);

            logger.LogInformation("Retrieved {Count} contacts", contactList.Count);

            return contactList;
        }

        public async Task<ContactDto?> GetContactByIdAsync(Guid id)
        {
            logger.LogDebug("Retrieving contact with ID: {ContactId}", id);

            var contact = await contactRepository.GetById(id);
            if (contact is null)
            {
                logger.LogWarning("Contact not found with ID: {ContactId}", id);
                return null;
            }

            logger.LogDebug("Contact {ContactId} retrieved successfully", id);

            return mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto?> UpdateContactAsync(Guid id, UpdateContactRequestDto request)
        {
            logger.LogInformation("Updating contact with ID: {ContactId}", id);

            var existingContact = await contactRepository.GetById(id);
            if (existingContact is null)
            {
                logger.LogWarning("Contact not found for update with ID: {ContactId}", id);
                return null;
            }

            var oldEmail = existingContact.Email;
            var oldPhone = existingContact.Phone;

            existingContact.Email = request.Email;
            existingContact.Phone = request.Phone;

            var updatedContact = await contactRepository.UpdateAsync(existingContact);
            if (updatedContact is null)
            {
                logger.LogError("Failed to update contact with ID: {ContactId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Contact {ContactId} updated successfully - Email: {OldEmail} → {NewEmail}, Phone: {OldPhone} → {NewPhone}",
                id, oldEmail, updatedContact.Email, oldPhone, updatedContact.Phone);

            return mapper.Map<ContactDto>(updatedContact);
        }

        public async Task<string?> DeleteContactAsync(Guid id)
        {
            logger.LogInformation("Deleting contact with ID: {ContactId}", id);

            var contact = await contactRepository.DeleteAsync(id);
            if (contact is null)
            {
                logger.LogWarning("Contact not found for deletion with ID: {ContactId}", id);
                return null;
            }

            logger.LogInformation("Contact {ContactId} marked for deletion (pending transaction commit)", id);

            return $"Contact with ID {contact.Id} deleted successfully!";
        }
    }
}