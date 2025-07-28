using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace MedNet.API.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;
        private readonly IInsuranceService insuranceService;
        private readonly IMedicationService medicationService;
        private readonly IMedicalFileService medicalFileService;
        private readonly IUserManagementService userManagementService;

        public PatientService(IPatientRepository patientRepository,
            IAddressService addressService,
            IContactService contactService,
            IInsuranceService insuranceService,
            IMedicationService medicationService,
            IMedicalFileService medicalFileService,
            IUserManagementService userManagementService)
        {
            this.patientRepository = patientRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.insuranceService = insuranceService;
            this.medicationService = medicationService;
            this.medicalFileService = medicalFileService;
            this.userManagementService = userManagementService;
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
                    UserId = request.UserId.Trim(),
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
                    UserId = patient.UserId,
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

        public async Task<IEnumerable<PatientBasicSummaryDto>> GetAllPatientsAsync()
        {
            var patients = await patientRepository.GetAllAsync();
            var patientDtos = new List<PatientBasicSummaryDto>();

            foreach (var patient in patients)
            {
                var addressDto = new AddressDto
                {
                    Id = patient.AddressId,
                    Street = patient.Address?.Street,
                    StreetNr = patient.Address?.StreetNr ?? 0,
                    City = patient.Address?.City,
                    State = patient.Address?.State,
                    Country = patient.Address?.Country,
                    PostalCode = patient.Address?.PostalCode

                };

                var contactDto = new ContactDto
                {
                    Id = patient.ContactId,
                    Phone = patient.Contact?.Phone,
                    Email = patient.Contact?.Email
                };

                patientDtos.Add(new PatientBasicSummaryDto
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
                });
            }

            return patientDtos;
        }

        public async Task<PatientDto?> GetPatientByUserIdAsync(string userId)
        {
            Console.WriteLine($"[DEBUG] Searching for patient with UserId: {userId}");

            var patient = await patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
            {
                Console.WriteLine($"[ERROR] No patient found for UserId: {userId}");
                return null;
            }

            var addressDto = await addressService.GetAddressByIdAsync(patient.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(patient.ContactId);

            var insurances = (await insuranceService.GetInsurancesByPatientIdAsync(patient.Id))
                .Select(i => new DisplayInsuranceDto
                {
                    Id = i.Id,
                    Provider = i.Provider,
                    PolicyNumber = i.PolicyNumber,
                    CoverageStartDate = i.CoverageStartDate,
                    CoverageEndDate = i.CoverageEndDate
                }).ToList();  

            var medications = (await medicationService.GetMedicationsByPatientIdAsync(patient.Id))
                .Select(m => new DisplayMedicationDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList();

            var medicalFiles = (await medicalFileService.GetMedicalFilesByPatientIdAsync(patient.Id))
                .Select(mf => new DisplayMedicalFileDto
                {
                    Id = mf.Id,
                    FileName = mf.FileName,
                    FileType = mf.FileType,
                    FilePath = mf.FilePath,
                    DateUploaded = mf.DateUploaded
                }).ToList();

            Console.WriteLine($"[SUCCESS] Found patient: {patient.FirstName} {patient.LastName}");

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
                Insurances = insurances, 
                Medications = medications,
                MedicalFiles = medicalFiles
            };
        }



        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await patientRepository.GetById(id);
            if (patient == null) return null;

            var addressDto = await addressService.GetAddressByIdAsync(patient.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(patient.ContactId);
            var insurances = (await insuranceService.GetInsurancesByPatientIdAsync(patient.Id))
                .Select(i => new DisplayInsuranceDto
                {
                    Id = i.Id,
                    Provider = i.Provider,
                    PolicyNumber = i.PolicyNumber,
                    CoverageStartDate = i.CoverageStartDate,
                    CoverageEndDate = i.CoverageEndDate
                }).ToList();

            var medications = (await medicationService.GetMedicationsByPatientIdAsync(patient.Id))
                .Select(m => new DisplayMedicationDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList();

            var medicalFiles = (await medicalFileService.GetMedicalFilesByPatientIdAsync(patient.Id))
                .Select(mf => new DisplayMedicalFileDto
                {
                    Id = mf.Id,
                    FileName = mf.FileName,
                    FileType = mf.FileType,
                    FilePath = mf.FilePath,
                    DateUploaded = mf.DateUploaded
                }).ToList();

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
                Insurances = insurances,
                Medications = medications,
                MedicalFiles = medicalFiles
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

                if (!string.IsNullOrEmpty(patient.UserId))
                {
                    var identityResult = await userManagementService.DeleteUserByIdAsync(patient.UserId);
                    if (!identityResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        throw new CustomException("Failed to delete associated Identity user: " +
                            string.Join(", ", identityResult.Errors.Select(e => e.Description)));
                    }
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
