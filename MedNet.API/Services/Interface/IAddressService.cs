using MedNet.API.Models.DTO;

namespace MedNet.API.Services
{
    public interface IAddressService
    {
        Task<AddressDto> CreateAddressAsync(CreateAddressRequestDto request);
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto?> GetAddressByIdAsync(Guid id);
    }
}
