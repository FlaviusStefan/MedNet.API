using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

using MedNet.API.Repositories.Interface;


namespace MedNet.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository addressRepository;
        private readonly ILogger<AddressService> logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger)
        {
            this.addressRepository = addressRepository;
            this.logger = logger;
        }
        public async Task<AddressDto> CreateAddressAsync(CreateAddressRequestDto request)
        {
            logger.LogInformation("Creating new address with Street: {Street}, StreeNr: {StreetNr}, City: {City}, State: {State}, Country: {Country}, Postal Code: {PostalCode}",
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

            logger.LogInformation("Address {AddressId} created succesfully", address.Id);

            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                StreetNr = address.StreetNr,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            };
        }
        public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
        {
            logger.LogInformation("Retrieving all addresses");

            var addresses = await addressRepository.GetAllAsync();

            var addressList = addresses.Select(address => new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                StreetNr = address.StreetNr,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            }).ToList();

            logger.LogInformation("Retrieved {Count} addresses", addressList.Count);

            return addressList;
        }

        public async Task<AddressDto?> GetAddressByIdAsync(Guid id)
        {
            logger.LogDebug("Retrieving address with ID: {AddressId}", id);

            var address = await addressRepository.GetById(id);
            if (address == null)
            {
                logger.LogWarning("Address not found with ID: {AddressId}", id);
                return null;
            }

            logger.LogDebug("Address {AddressId} retrieved successfully", id);

            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                StreetNr = address.StreetNr,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            };
        }

        public async Task<AddressDto?> UpdateAddressAsync(Guid id, UpdateAddressRequestDto request)
        {
            logger.LogInformation("Updating address with ID: {AddressId}", id);

            var existingAddress = await addressRepository.GetById(id);

            if (existingAddress == null)
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

            if (updatedAddress == null)
            {
                logger.LogError("Failed to update address with ID: {AddressId}", id);
                return null;
            }

            logger.LogInformation("Address {AddressId} updated successfully", id);

            return new AddressDto
            {
                Id = updatedAddress.Id,
                Street = updatedAddress.Street,
                StreetNr = updatedAddress.StreetNr,
                City = updatedAddress.City,
                State = updatedAddress.State,
                Country = updatedAddress.Country,
                PostalCode = updatedAddress.PostalCode
            };
        }

        public async Task<string?> DeleteAddressAsync(Guid id)
        {
            logger.LogInformation("Deleting address with ID: {AddressId}", id);

            var address = await addressRepository.DeleteAsync(id);
            if (address == null)
            {
                logger.LogWarning("Address not found for deletion with ID: {AddressId}", id);
                return null;
            }

            logger.LogInformation("Address {AddressId} deleted successfully", id);

            return $"Address with ID {address.Id} deleted successfully!";
        }
    }
}
