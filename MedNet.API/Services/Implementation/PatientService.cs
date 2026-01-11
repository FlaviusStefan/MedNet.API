using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System.Numerics;
using System.Transactions;

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
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<PatientService> logger;

        public PatientService(IPatientRepository patientRepository,
            IAddressService addressService,
            IContactService contactService,
            IInsuranceService insuranceService,
            IMedicationService medicationService,
            IMedicalFileService medicalFileService,
            IUserManagementService userManagementService,
            ILogger<PatientService> logger,
            IUnitOfWork unitOfWork)
        {
            this.patientRepository = patientRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.insuranceService = insuranceService;
            this.medicationService = medicationService;
            this.medicalFileService = medicalFileService;
            this.userManagementService = userManagementService;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }


        public async Task<CreatedPatientDto> CreatePatientAsync(CreatePatientRequestDto request)
        {
            logger.LogInformation("Creating patient: {FirstName} {LastName}, UserId: {UserId}",
                request.FirstName, request.LastName, request.UserId);

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

                await unitOfWork.SaveChangesAsync();

                logger.LogInformation("Patient {PatientId} created successfully - {FirstName} {LastName}, UserId: {UserId}",
                    patient.Id, patient.FirstName, patient.LastName, patient.UserId);

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
                logger.LogError(ex, "Failed to create patient {FirstName} {LastName} with UserId: {UserId}",
                    request.FirstName, request.LastName, request.UserId);
                throw; 
            }
        }

        public async Task<IEnumerable<PatientBasicSummaryDto>> GetAllPatientsAsync()
        {
            logger.LogInformation("Retrieving all patients");

            var patients = await patientRepository.GetAllAsync();
            var patientDtos = new List<PatientBasicSummaryDto>();

            foreach (var patient in patients)
            {
                var addressDto = new AddressResponseDto
                {
                    Street = patient.Address?.Street,
                    StreetNr = patient.Address?.StreetNr ?? 0,
                    City = patient.Address?.City,
                    State = patient.Address?.State,
                    Country = patient.Address?.Country,
                    PostalCode = patient.Address?.PostalCode

                };

                var contactDto = new ContactResponseDto
                {
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

            logger.LogInformation("Retrieved {Count} patients", patientDtos.Count);

            return patientDtos;
        }

        public async Task<PatientResponseDto?> GetPatientByUserIdAsync(string userId)
        {
            logger.LogInformation("Searching for patient with UserId: {UserId}", userId);

            var patient = await patientRepository.GetByUserIdAsync(userId);
            
            if (patient == null)
            {
                logger.LogWarning("No patient found for UserId: {UserId}", userId);
                return null;
            }

            logger.LogInformation("Successfully retrieved patient {PatientName} (ID: {PatientId}) for UserId: {UserId} with {InsuranceCount} insurances, {MedicationCount} medications, {FileCount} files",
                $"{patient.FirstName} {patient.LastName}", patient.Id, userId,
                patient.Insurances.Count, patient.Medications.Count, patient.MedicalFiles.Count);

            return new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Height = patient.Height,
                Weight = patient.Weight,
                Address = patient.Address != null ? new AddressResponseDto
                {
                    Street = patient.Address.Street,
                    StreetNr = patient.Address.StreetNr,
                    City = patient.Address.City,
                    State = patient.Address.State,
                    PostalCode = patient.Address.PostalCode,
                    Country = patient.Address.Country
                } : null,
                Contact = patient.Contact != null ? new ContactResponseDto
                {
                    Phone = patient.Contact.Phone,
                    Email = patient.Contact.Email
                } : null,
                Insurances = patient.Insurances.Select(i => new DisplayInsuranceDto
                {
                    Id = i.Id,
                    Provider = i.Provider,
                    PolicyNumber = i.PolicyNumber,
                    CoverageStartDate = i.CoverageStartDate,
                    CoverageEndDate = i.CoverageEndDate
                }).ToList(),
                Medications = patient.Medications.Select(m => new DisplayMedicationDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList(),
                MedicalFiles = patient.MedicalFiles.Select(mf => new DisplayMedicalFileDto
                {
                    Id = mf.Id,
                    FileName = mf.FileName,
                    FileType = mf.FileType,
                    FilePath = mf.FilePath,
                    DateUploaded = mf.DateUploaded
                }).ToList()
            };
        }



        public async Task<PatientResponseDto?> GetPatientByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving patient with ID: {PatientId}", id);

            var patient = await patientRepository.GetById(id);
            if (patient == null)
            {
                logger.LogWarning("Patient not found with ID: {PatientId}", id);
                return null;
            }

            logger.LogInformation("Successfully retrieved patient {PatientName} (ID: {PatientId}) with {InsuranceCount} insurances, {MedicationCount} medications, {FileCount} files",
                $"{patient.FirstName} {patient.LastName}", patient.Id, patient.Insurances.Count, patient.Medications.Count, patient.MedicalFiles.Count);

            return new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Height = patient.Height,
                Weight = patient.Weight,
                Address = patient.Address != null ? new AddressResponseDto
                {
                    Street = patient.Address.Street,
                    StreetNr = patient.Address.StreetNr,
                    City = patient.Address.City,
                    State = patient.Address.State,
                    PostalCode = patient.Address.PostalCode,
                    Country = patient.Address.Country
                } : null,
                Contact = patient.Contact != null ? new ContactResponseDto
                {
                    Phone = patient.Contact.Phone,
                    Email = patient.Contact.Email
                } : null,
                Insurances = patient.Insurances.Select(i => new DisplayInsuranceDto
                {
                    Id = i.Id,
                    Provider = i.Provider,
                    PolicyNumber = i.PolicyNumber,
                    CoverageStartDate = i.CoverageStartDate,
                    CoverageEndDate = i.CoverageEndDate
                }).ToList(),
                Medications = patient.Medications.Select(m => new DisplayMedicationDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList(),
                MedicalFiles = patient.MedicalFiles.Select(mf => new DisplayMedicalFileDto
                {
                    Id = mf.Id,
                    FileName = mf.FileName,
                    FileType = mf.FileType,
                    FilePath = mf.FilePath,
                    DateUploaded = mf.DateUploaded
                }).ToList()
            };
        }


        public async Task<UpdatedPatientDto?> UpdatePatientAsync(Guid id, UpdatePatientRequestDto request)
        {
            logger.LogInformation("Updating patient with ID: {PatientId}", id);

            var existingPatient = await patientRepository.GetById(id);

            if (existingPatient == null)
            {
                logger.LogWarning("Patient not found for update with ID: {PatientId}", id);
                return null;
            }

            var oldHeight = existingPatient.Height;
            var oldWeight = existingPatient.Weight;

            existingPatient.FirstName = request.FirstName;
            existingPatient.LastName = request.LastName;
            existingPatient.DateOfBirth = request.DateOfBirth;
            existingPatient.Gender = request.Gender;
            existingPatient.Height = request.Height;
            existingPatient.Weight = request.Weight;

            var updatedPatient = await patientRepository.UpdateAsync(existingPatient);

            if (updatedPatient == null)
            {
                logger.LogError("Failed to update patient with ID: {PatientId}", id);
                return null;
            }

            logger.LogInformation("Patient {PatientId} updated successfully - {FirstName} {LastName}, Height: {OldHeight} → {NewHeight}, Weight: {OldWeight} → {NewWeight}",
                id, updatedPatient.FirstName, updatedPatient.LastName, oldHeight, updatedPatient.Height, oldWeight, updatedPatient.Weight);


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
            logger.LogInformation("Attempting to delete patient with ID: {PatientId}", id);

            if (id == Guid.Empty)
            {
                logger.LogWarning("Delete attempt with invalid empty GUID");
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var patient = await patientRepository.GetById(id);
            if (patient == null)
            {
                logger.LogWarning("Patient not found for deletion with ID: {PatientId}", id);
                return null;
            }

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                },
                TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                logger.LogDebug("Deleting patient {PatientId} - {FirstName} {LastName}, UserId: {UserId}",
                    id, patient.FirstName, patient.LastName, patient.UserId);

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
                        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                        logger.LogError("Failed to delete Identity user for patient {PatientId}: {Errors}", id, errors);
                        throw new CustomException("Failed to delete associated Identity user: " + errors);
                    }
                }

                scope.Complete();

                logger.LogInformation("Patient {PatientId} deleted successfully - {FirstName} {LastName}, UserId: {UserId}",
                    id, patient.FirstName, patient.LastName, patient.UserId);

                return "Patient has been deleted successfully!";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete patient with ID: {PatientId}", id);
                throw new CustomException("An unexpected error occurred while deleting the patient: " + ex.Message, ex);
            }
        }
    }
}
