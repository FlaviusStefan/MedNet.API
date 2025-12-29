using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using System.Numerics;

namespace MedNet.API.Services.Implementation
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository hospitalRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;

        public HospitalService(IHospitalRepository hospitalRepository, IAddressService addressService, IContactService contactService)
        {
            this.hospitalRepository = hospitalRepository;
            this.addressService = addressService;
            this.contactService = contactService;
        }
        public async Task<HospitalDto> CreateHospitalAsync(CreateHospitalRequestDto request)
        {
            using var transaction = await hospitalRepository.BeginTransactionAsync();

            try
            {
                var addressDto = await addressService.CreateAddressAsync(request.Address);
                var contactDto = await contactService.CreateContactAsync(request.Contact);
                var hospital = new Hospital
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id,
                };

                await hospitalRepository.CreateAsync(hospital);

                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred: " + ex.Message, ex);
            }
            
        }

        public async Task<IEnumerable<HospitalResponseDto>> GetAllHospitalsAsync()
        {
            var hospitals = await hospitalRepository.GetAllAsync();

            var result = new List<HospitalResponseDto>();

            foreach (var hospital in hospitals)
            {
                // Load address
                var addressDto = await addressService.GetAddressByIdAsync(hospital.AddressId);
                AddressResponseDto? addressResponse = null;
                if (addressDto != null)
                {
                    addressResponse = new AddressResponseDto
                    {
                        Street = addressDto.Street,
                        StreetNr = addressDto.StreetNr,
                        City = addressDto.City,
                        State = addressDto.State,
                        PostalCode = addressDto.PostalCode,
                        Country = addressDto.Country
                    };
                }

                // Load contact
                var contactDto = await contactService.GetContactByIdAsync(hospital.ContactId);
                ContactResponseDto? contactResponse = null;
                if (contactDto != null)
                {
                    contactResponse = new ContactResponseDto
                    {
                        Phone = contactDto.Phone,
                        Email = contactDto.Email
                    };
                }

                result.Add(new HospitalResponseDto
                {
                    Id = hospital.Id,
                    Name = hospital.Name,
                    Address = addressResponse,
                    Contact = contactResponse
                });
            }

            return result;
        }

        public async Task<HospitalResponseDto?> GetHospitalByIdAsync(Guid id)
        {
            var hospital = await hospitalRepository.GetById(id);

            if (hospital == null) return null;
            var addressDto = await addressService.GetAddressByIdAsync(hospital.AddressId);
            AddressResponseDto? addressResponse = null;
            if (addressDto != null)
            {
                addressResponse = new AddressResponseDto
                {
                    Street = addressDto.Street,
                    StreetNr = addressDto.StreetNr,
                    City = addressDto.City,
                    State = addressDto.State,
                    PostalCode = addressDto.PostalCode,
                    Country = addressDto.Country
                };
            }
            var contactDto = await contactService.GetContactByIdAsync(hospital.ContactId);
            ContactResponseDto? contactResponse = null;
            if (contactDto != null)
            {
                contactResponse = new ContactResponseDto
                {
                    Phone = contactDto.Phone,
                    Email = contactDto.Email
                };
            }


            return new HospitalResponseDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = addressResponse,
                Contact = contactResponse
            };
        }

        public async Task<HospitalDto?> UpdateHospitalAsync(Guid id, UpdateHospitalRequestDto request)
        {
            var existingHospital = await hospitalRepository.GetById(id);

            if (existingHospital == null) return null;

            existingHospital.Name = request.Name;

            
            var updatedHospital = await hospitalRepository.UpdateAsync(existingHospital);

            if (updatedHospital == null) return null;

            return new HospitalDto
            {
                Id = updatedHospital.Id,
                Name = updatedHospital.Name,
            };
        }

        public async Task<string?> DeleteHospitalAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var hospital = await hospitalRepository.GetById(id);
            if (hospital == null)
            {
                return null;
            }

            using var transaction = await hospitalRepository.BeginTransactionAsync();

            try
            {
                // First, delete the hospital entity
                await hospitalRepository.DeleteAsync(id);

                // Then, delete related entities if they exist
                if (hospital.Address != null)
                {
                    await addressService.DeleteAddressAsync(hospital.Address.Id);
                }

                if (hospital.Contact != null)
                {
                    await contactService.DeleteContactAsync(hospital.Contact.Id);
                }

                // Commit the transaction
                await transaction.CommitAsync();

                return "Hospital deleted successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred while deleting the hospital: " + ex.Message, ex);
            }
        }

    }
}
