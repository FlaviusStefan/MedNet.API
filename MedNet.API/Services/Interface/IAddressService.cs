using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IAddressService
    {
        Task<AddressDto> CreateAddressAsync(CreateAddressRequestDto request);
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto?> GetAddressByIdAsync(Guid id);
        Task<AddressDto?> UpdateAddressAsync(Guid id, UpdateAddressRequestDto request);
        Task<string?> DeleteAddressAsync(Guid id);
    }
}
