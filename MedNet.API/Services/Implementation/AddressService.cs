using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;

using MedNet.API.Repositories.Interface;


namespace MedNet.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            this.addressRepository = addressRepository;
        }
        public async Task<AddressDto> CreateAddressAsync(CreateAddressRequestDto request)
        {
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
            var addresses = await addressRepository.GetAllAsync();

            return addresses.Select(address => new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                StreetNr = address.StreetNr,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            }).ToList();
        }

        public async Task<AddressDto?> GetAddressByIdAsync(Guid id)
        {
            var address = await addressRepository.GetById(id);
            if (address == null)
            {
                return null;
            }

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
            var existingAddress = await addressRepository.GetById(id);

            if (existingAddress == null) return null;

            existingAddress.Street = request.Street;
            existingAddress.StreetNr = request.StreetNr;
            existingAddress.City = request.City;
            existingAddress.State = request.State;
            existingAddress.Country = request.Country;
            existingAddress.PostalCode = request.PostalCode;

            var updatedAddress = await addressRepository.UpdateAsync(existingAddress);

            if (updatedAddress == null) return null;

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

        public async Task<AddressDto> DeleteAddressAsync(Guid id)
        {
            var address = await addressRepository.DeleteAsync(id);
            if (address == null) return null;

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
    }
}
