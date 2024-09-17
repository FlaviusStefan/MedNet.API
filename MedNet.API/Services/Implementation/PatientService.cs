using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
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
        private readonly IInsuranceService insuranceService;

        public PatientService(IPatientRepository patientRepository, 
            IAddressService addressService, 
            IContactService contactService,
            IInsuranceService insuranceService)
        {
            this.patientRepository = patientRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.insuranceService = insuranceService;
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

            var insuranceDtos = await insuranceService.GetAllInsurancesAsync();
            var addressDtos = await addressService.GetAllAddressesAsync();
            var contactDtos = await contactService.GetAllContactsAsync();

            return patients.Select(patient => new PatientDto
            { 
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                Height = patient.Height, 
                Weight = patient.Weight,
                Address = addressDtos.FirstOrDefault(a => a.Id == patient.AddressId),
                Contact = contactDtos.FirstOrDefault(c => c.Id == patient.ContactId),
                Insurances = insuranceDtos
                    .Where(dto => patient.Insurances.Select(i => i.Id).Contains(dto.Id))
                    .Select(dto => new InsuranceDto
                    {
                        Id = dto.Id,
                        Provider = dto.Provider,
                        PolicyNumber = dto.PolicyNumber,
                        CoverageStartDate = dto.CoverageStartDate,
                        CoverageEndDate = dto.CoverageEndDate
                    }).ToList()
            });
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await patientRepository.GetById(id);

            if (patient == null) return null;

            var insuranceDtos = await insuranceService.GetAllInsurancesAsync();
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
                Contact = contactDto,
                Insurances = insuranceDtos
                    .Where(dto => patient.Insurances.Select(i => i.Id).Contains(dto.Id))
                    .Select(dto => new InsuranceDto
                    {
                        Id = dto.Id,
                        Provider = dto.Provider,
                        PolicyNumber = dto.PolicyNumber,
                        CoverageStartDate = dto.CoverageStartDate,
                        CoverageEndDate = dto.CoverageEndDate
                    }).ToList()
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
        public async Task<string?> DeletePatientAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var patient = await patientRepository.GetById(id);
            if (patient == null)
            {
                return null;
            }

            using var transaction = await patientRepository.BeginTransactionAsync();

            try
            {
                await patientRepository.DeleteAsync(id);

                if (patient.Address != null)
                {
                    await addressService.DeleteAddressAsync(patient.Address.Id);
                }

                if (patient.Contact != null)
                {
                    await contactService.DeleteContactAsync(patient.Contact.Id);
                }


                await transaction.CommitAsync();

                return "Patient has been deleted successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred while deleting the patient: " + ex.Message, ex);
            }
        }
    }
}
