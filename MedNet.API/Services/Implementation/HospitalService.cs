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

        public async Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync()
        {
            var hospitals = await hospitalRepository.GetAllAsync();

            var addressDtos = await addressService.GetAllAddressesAsync();
            var contactDtos = await contactService.GetAllContactsAsync();

            return hospitals.Select(hospital => new HospitalDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = addressDtos.FirstOrDefault(a => a.Id == hospital.AddressId),
                Contact = contactDtos.FirstOrDefault(c => c.Id == hospital.ContactId),

            }).ToList();
        }

        public async Task<HospitalDto?> GetHospitalByIdAsync(Guid id)
        {
            var hospital = await hospitalRepository.GetById(id);

            if (hospital == null) return null;
            var addressDto = await addressService.GetAddressByIdAsync(hospital.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(hospital.ContactId);

            return new HospitalDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = addressDto,
                Contact = contactDto
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
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var hospital = await hospitalRepository.GetById(id);
            if(hospital == null)
            {
                return null;
            }

            using var transaction = await hospitalRepository.BeginTransactionAsync();

            try
            {
                if(hospital.Address != null)
                {
                    await addressService.DeleteAddressAsync(hospital.Address.Id);
                }

                if (hospital.Contact != null)
                {
                    await contactService.DeleteContactAsync(hospital.Contact.Id);
                }

                await hospitalRepository.DeleteAsync(id);

                await transaction.CommitAsync();

                return "Hospital deleted succesfully!";

            } catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred while deleting the hospital: " + ex.Message, ex);
            }
        }
    }
}
