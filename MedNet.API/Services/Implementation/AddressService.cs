using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository addressRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<AddressService> logger;

        public AddressService(IAddressRepository addressRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddressService> logger)
        {
            this.addressRepository = addressRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<AddressDto> CreateAddressAsync(CreateAddressRequestDto request)
        {
            logger.LogInformation("Creating new address with Street: {Street}, StreetNr: {StreetNr}, City: {City}, State: {State}, Country: {Country}, Postal Code: {PostalCode}",
                request.Street, request.StreetNr, request.City, request.State, request.Country, request.PostalCode);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                Street = request.Street,
                StreetNr = request.StreetNr,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode
            };

            await addressRepository.CreateAsync(address);

            logger.LogInformation("Address {AddressId} created (pending transaction commit)", address.Id);

            return mapper.Map<AddressDto>(address);
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
        {
            logger.LogInformation("Retrieving all addresses");

            var addresses = await addressRepository.GetAllAsync();
            var addressList = mapper.Map<List<AddressDto>>(addresses);

            logger.LogInformation("Retrieved {Count} addresses", addressList.Count);

            return addressList;
        }

        public async Task<AddressDto?> GetAddressByIdAsync(Guid id)
        {
            logger.LogDebug("Retrieving address with ID: {AddressId}", id);

            var address = await addressRepository.GetById(id);
            if (address is null)
            {
                logger.LogWarning("Address not found with ID: {AddressId}", id);
                return null;
            }

            logger.LogDebug("Address {AddressId} retrieved successfully", id);

            return mapper.Map<AddressDto>(address);
        }

        public async Task<AddressDto?> UpdateAddressAsync(Guid id, UpdateAddressRequestDto request)
        {
            logger.LogInformation("Updating address with ID: {AddressId}", id);

            var existingAddress = await addressRepository.GetById(id);
            if (existingAddress is null)
            {
                logger.LogWarning("Address not found for update with ID: {AddressId}", id);
                return null;
            }

            existingAddress.Street = request.Street;
            existingAddress.StreetNr = request.StreetNr;
            existingAddress.City = request.City;
            existingAddress.State = request.State;
            existingAddress.Country = request.Country;
            existingAddress.PostalCode = request.PostalCode;

            var updatedAddress = await addressRepository.UpdateAsync(existingAddress);
            if (updatedAddress is null)
            {
                logger.LogError("Failed to update address with ID: {AddressId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Address {AddressId} updated successfully", id);

            return mapper.Map<AddressDto>(updatedAddress);
        }

        public async Task<string?> DeleteAddressAsync(Guid id)
        {
            logger.LogInformation("Deleting address with ID: {AddressId}", id);

            var address = await addressRepository.DeleteAsync(id);
            if (address is null)
            {
                logger.LogWarning("Address not found for deletion with ID: {AddressId}", id);
                return null;
            }

            logger.LogInformation("Address {AddressId} marked for deletion (pending transaction commit)", id);

            return $"Address with ID {address.Id} deleted successfully!";
        }
    }
}