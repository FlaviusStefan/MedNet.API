using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using System.Numerics;

namespace MedNet.API.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;

        public PatientService(IPatientRepository patientRepository, IAddressService addressService, IContactService contactService)
        {
            this.patientRepository = patientRepository;
            this.addressService = addressService;
            this.contactService = contactService;
        }


        public async Task<CreatedPatientDto> CreatePatientAsync(CreatePatientRequestDto request)
        {
            using var transaction = await patientRepository.BeginTransactionAsync();

            try
            {
                var addressDto = await addressService.CreateAddressAsync(request.Address);

                var contactDto = await contactService.CreateContactAsync(request.Contact);

                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    Height = request.Height,
                    Weight = request.Weight,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id
                };

                await patientRepository.CreateAsync(patient);

                await transaction.CommitAsync();

                return new CreatedPatientDto
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender,
                    Height = patient.Height,
                    Weight = patient.Weight,
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


        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await patientRepository.GetAllAsync();
            var addressDtos = await addressService.GetAllAddressesAsync();
            var contactDtos = await contactService.GetAllContactsAsync();

            return patients.Select(patient => new PatientDto
            { 
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                Address = addressDtos.FirstOrDefault(a => a.Id == patient.AddressId),
                Contact = contactDtos.FirstOrDefault(c => c.Id == patient.ContactId),
            });
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await patientRepository.GetById(id);

            if (patient == null) return null;

            var addressDto = await addressService.GetAddressByIdAsync(patient.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(patient.ContactId);

            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Height = patient.Height,
                Weight = patient.Weight,
                Address = addressDto,
                Contact = contactDto
            };
        }

        public async Task<UpdatedPatientDto?> UpdatePatientAsync(Guid id, UpdatePatientRequestDto request)
        {
            var existingPatient = await patientRepository.GetById(id);

            if(existingPatient == null) return null;

            existingPatient.FirstName = request.FirstName;
            existingPatient.LastName = request.LastName;
            existingPatient.DateOfBirth = request.DateOfBirth;
            existingPatient.Gender = request.Gender;
            existingPatient.Height = request.Height;
            existingPatient.Weight = request.Weight;

            var updatedPatient = await patientRepository.UpdateAsync(existingPatient);

            if (updatedPatient == null) return null;

            return new UpdatedPatientDto
            {
                Id = updatedPatient.Id,
                FirstName = updatedPatient.FirstName,
                LastName = updatedPatient.LastName,
                DateOfBirth = updatedPatient.DateOfBirth,
                Gender = updatedPatient.Gender,
                Height = updatedPatient.Height,
                Weight = updatedPatient.Weight
            };
        }
        public Task<string?> DeletePatientAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
