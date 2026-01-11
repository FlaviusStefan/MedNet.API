using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using System.Transactions;

namespace MedNet.API.Services.Implementation
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository hospitalRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;
        private readonly IUserManagementService userManagementService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HospitalService> logger;

        public HospitalService(
            IHospitalRepository hospitalRepository, 
            IAddressService addressService, 
            IContactService contactService, 
            ILogger<HospitalService> logger, 
            IUserManagementService userManagementService, 
            IUnitOfWork unitOfWork)
        {
            this.hospitalRepository = hospitalRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.logger = logger;
            this.userManagementService = userManagementService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<HospitalDto> CreateHospitalAsync(CreateHospitalRequestDto request)
        {
            logger.LogInformation("Creating hospital: {HospitalName}, UserId: {UserId}",
                request.Name, request.UserId);

            try
            {
                var addressDto = await addressService.CreateAddressAsync(request.Address);
                var contactDto = await contactService.CreateContactAsync(request.Contact);

                var hospital = new Hospital
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    UserId = request.UserId,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id,
                };

                await hospitalRepository.CreateAsync(hospital);
                await unitOfWork.SaveChangesAsync();

                logger.LogInformation("Hospital {HospitalId} created successfully - {HospitalName}",
                    hospital.Id, hospital.Name);

                return new HospitalDto
                {
                    Id = hospital.Id,
                    Name = hospital.Name,
                    Address = addressDto,
                    Contact = contactDto
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create hospital {HospitalName} with UserId: {UserId}",
                    request.Name, request.UserId);
                throw; 
            }
        }

        public async Task<IEnumerable<HospitalResponseDto>> GetAllHospitalsAsync()
        {
            logger.LogInformation("Retrieving all hospitals");

            var hospitals = await hospitalRepository.GetAllAsync();

            var result = hospitals.Select(hospital => new HospitalResponseDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = hospital.Address != null ? new AddressResponseDto
                {
                    Street = hospital.Address.Street,
                    StreetNr = hospital.Address.StreetNr,
                    City = hospital.Address.City,
                    State = hospital.Address.State,
                    PostalCode = hospital.Address.PostalCode,
                    Country = hospital.Address.Country
                } : null,
                Contact = hospital.Contact != null ? new ContactResponseDto
                {
                    Phone = hospital.Contact.Phone,
                    Email = hospital.Contact.Email
                } : null
            }).ToList();

            logger.LogInformation("Retrieved {Count} hospitals", result.Count);

            return result;
        }

        public async Task<HospitalResponseDto?> GetHospitalByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving hospital with ID: {HospitalId}", id);

            var hospital = await hospitalRepository.GetById(id);

            if (hospital == null)
            {
                logger.LogWarning("Hospital not found with ID: {HospitalId}", id);
                return null;
            }

            logger.LogInformation("Hospital {HospitalId} retrieved - {HospitalName}",
                hospital.Id, hospital.Name);

            return new HospitalResponseDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = hospital.Address != null ? new AddressResponseDto
                {
                    Street = hospital.Address.Street,
                    StreetNr = hospital.Address.StreetNr,
                    City = hospital.Address.City,
                    State = hospital.Address.State,
                    PostalCode = hospital.Address.PostalCode,
                    Country = hospital.Address.Country
                } : null,
                Contact = hospital.Contact != null ? new ContactResponseDto
                {
                    Phone = hospital.Contact.Phone,
                    Email = hospital.Contact.Email
                } : null
            };
        }

        public async Task<HospitalDto?> UpdateHospitalAsync(Guid id, UpdateHospitalRequestDto request)
        {
            logger.LogInformation("Updating hospital with ID: {HospitalId}", id);

            var existingHospital = await hospitalRepository.GetById(id);

            if (existingHospital == null)
            {
                logger.LogWarning("Hospital not found for update with ID: {HospitalId}", id);
                return null;
            }

            var oldName = existingHospital.Name;
            existingHospital.Name = request.Name;

            var updatedHospital = await hospitalRepository.UpdateAsync(existingHospital);

            if (updatedHospital == null)
            {
                logger.LogError("Failed to update hospital with ID: {HospitalId}", id);
                return null;
            }

            logger.LogInformation("Hospital {HospitalId} updated successfully - Name: '{OldName}' → '{NewName}'",
                id, oldName, updatedHospital.Name);

            return new HospitalDto
            {
                Id = updatedHospital.Id,
                Name = updatedHospital.Name,
            };
        }

        public async Task<string?> DeleteHospitalAsync(Guid id)
        {
            logger.LogInformation("Attempting to delete hospital with ID: {HospitalId}", id);

            if (id == Guid.Empty)
            {
                logger.LogWarning("Delete attempt with invalid empty GUID");
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var hospital = await hospitalRepository.GetById(id);
            if (hospital == null)
            {
                logger.LogWarning("Hospital not found for deletion with ID: {HospitalId}", id);
                return null;
            }

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                { 
                    IsolationLevel = IsolationLevel.ReadCommitted 
                },
                TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                logger.LogDebug("Deleting hospital {HospitalId} - {HospitalName}",
                    id, hospital.Name);

                await hospitalRepository.DeleteAsync(id);

                if (hospital.Address != null)
                {
                    await addressService.DeleteAddressAsync(hospital.Address.Id);
                }

                if (hospital.Contact != null)
                {
                    await contactService.DeleteContactAsync(hospital.Contact.Id);
                }

                if (!string.IsNullOrEmpty(hospital.UserId))
                {
                    var identityResult = await userManagementService.DeleteUserByIdAsync(hospital.UserId);
                    if (!identityResult.Succeeded)
                    {
                        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                        logger.LogError("Failed to delete Identity user for hospital {HospitalId}: {Errors}", id, errors);
                        throw new CustomException("Failed to delete associated Identity user: " + errors);
                    }
                }

                await unitOfWork.SaveChangesAsync();

                scope.Complete();

                logger.LogInformation("Hospital {HospitalId} deleted successfully - {HospitalName}",
                    id, hospital.Name);

                return "Hospital deleted successfully!";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete hospital with ID: {HospitalId}", id);
                throw new CustomException("An unexpected error occurred while deleting the hospital: " + ex.Message, ex);
            }
        }
    }
}
