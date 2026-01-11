using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace MedNet.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;
        private readonly ISpecializationService specializationService;
        private readonly IQualificationService qualificationService;
        private readonly IUserManagementService userManagementService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DoctorService> logger;


        public DoctorService(
            IDoctorRepository doctorRepository,
            IAddressService addressService,
            IContactService contactService,
            ISpecializationService specializationService,
            IQualificationService qualificationService,
            IUserManagementService userManagementService,
            ILogger<DoctorService> logger,
            IUnitOfWork unitOfWork)
        {
            this.doctorRepository = doctorRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.specializationService = specializationService;
            this.qualificationService = qualificationService;
            this.userManagementService = userManagementService;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<CreatedDoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            logger.LogInformation("Creating doctor: {FirstName} {LastName}, License: {LicenseNumber}, UserId: {UserId}",
               request.FirstName, request.LastName, request.LicenseNumber, request.UserId);

            try
            {
                var validSpecializations = await specializationService.ValidateSpecializationsAsync(request.SpecializationIds);
                logger.LogDebug("Validated {Count} specializations for doctor {FirstName} {LastName}",
                    validSpecializations.Count, request.FirstName, request.LastName);

                var addressDto = await addressService.CreateAddressAsync(request.Address);
                var contactDto = await contactService.CreateContactAsync(request.Contact);

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserId = request.UserId.Trim(),
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    LicenseNumber = request.LicenseNumber,
                    YearsOfExperience = request.YearsOfExperience,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id,
                    DoctorSpecializations = validSpecializations.Keys.Select(sid => new DoctorSpecialization
                    {
                        DoctorId = Guid.NewGuid(),
                        SpecializationId = sid
                    }).ToList()
                };

                await doctorRepository.CreateAsync(doctor);

                var createdQualifications = new List<QualificationDto>();
                logger.LogDebug("Creating {Count} qualifications for doctor {DoctorId}",
                    request.Qualifications.Count, doctor.Id);

                foreach (var qualificationRequest in request.Qualifications)
                {
                    var qualificationDto = await qualificationService.CreateQualificationAsync(new CreateQualificationRequestDto
                    {
                        DoctorId = doctor.Id,
                        Degree = qualificationRequest.Degree,
                        Institution = qualificationRequest.Institution,
                        StudiedYears = qualificationRequest.StudiedYears,
                        YearOfCompletion = qualificationRequest.YearOfCompletion
                    },
                    autoSave: false);
                    createdQualifications.Add(qualificationDto);
                }

                logger.LogInformation("Created {Count} qualifications for doctor {DoctorId}",
                    createdQualifications.Count, doctor.Id);

                await unitOfWork.SaveChangesAsync();

                logger.LogInformation("Doctor {DoctorId} created successfully - {FirstName} {LastName}, License: {LicenseNumber}, with {QualCount} qualifications",
                    doctor.Id, doctor.FirstName, doctor.LastName, doctor.LicenseNumber, createdQualifications.Count);

                return new CreatedDoctorDto
                {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    DateOfBirth = doctor.DateOfBirth,
                    Gender = doctor.Gender,
                    LicenseNumber = doctor.LicenseNumber,
                    YearsOfExperience = doctor.YearsOfExperience,
                    Address = addressDto,
                    Contact = contactDto,
                    Specializations = validSpecializations.Values.ToList(),
                    Qualifications = createdQualifications
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create doctor {FirstName} {LastName} with UserId: {UserId}",
                    request.FirstName, request.LastName, request.UserId);
                throw; 
            }
        }


        public async Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync()
        {
            logger.LogInformation("Retrieving all doctors");

            var doctors = await doctorRepository.GetAllAsync();

            var doctorList = doctors.Select(doctor => new DoctorResponseDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = doctor.Address != null ? new AddressResponseDto
                {
                    Street = doctor.Address.Street,
                    StreetNr = doctor.Address.StreetNr,
                    City = doctor.Address.City,
                    State = doctor.Address.State,
                    PostalCode = doctor.Address.PostalCode,
                    Country = doctor.Address.Country
                } : null,

                Contact = doctor.Contact != null ? new ContactResponseDto
                {
                    Phone = doctor.Contact.Phone,
                    Email = doctor.Contact.Email,
                } : null,
                Specializations = doctor.DoctorSpecializations
                    .Select(ds => ds.Specialization.Name)
                    .ToList(),
                Qualifications = doctor.Qualifications
                    .Select(q => new QualificationResponseDto
                    {
                        Degree = q.Degree,
                        Institution = q.Institution,
                        StudiedYears = q.StudiedYears,
                        YearOfCompletion = q.YearOfCompletion
                    }).ToList()
            }).ToList();

            logger.LogInformation("Retrieved {Count} doctors", doctorList.Count);

            return doctorList;
        }

        public async Task<DoctorResponseDto?> GetDoctorByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving doctor with ID: {DoctorId}", id);

            var doctor = await doctorRepository.GetById(id);

            if (doctor == null)
            {
                logger.LogWarning("Doctor not found with ID: {DoctorId}", id);
                return null;
            }

            var specializationDtos = await specializationService.GetAllSpecializationsAsync();
            var qualificationDtos = await qualificationService.GetQualificationsByDoctorIdAsync(doctor.Id);
            var qualificationResponses = await qualificationService.GetQualificationsByDoctorIdAsync<QualificationResponseDto>(
                doctor.Id,
                q => new QualificationResponseDto
                {
                    Degree = q.Degree,
                    Institution = q.Institution,
                    StudiedYears = q.StudiedYears,
                    YearOfCompletion = q.YearOfCompletion
                });
            var addressDto = await addressService.GetAddressByIdAsync(doctor.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(doctor.ContactId);

            logger.LogInformation("Doctor {DoctorId} retrieved - {FirstName} {LastName}, License: {LicenseNumber}, Specializations: {SpecCount}",
                doctor.Id, doctor.FirstName, doctor.LastName, doctor.LicenseNumber, doctor.DoctorSpecializations.Count);

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = doctor.Address != null ? new AddressResponseDto
                {
                    Street = doctor.Address.Street,
                    StreetNr = doctor.Address.StreetNr,
                    City = doctor.Address.City,
                    State = doctor.Address.State,
                    PostalCode = doctor.Address.PostalCode,
                    Country = doctor.Address.Country
                } : null,
                Contact = doctor.Contact != null ? new ContactResponseDto
                {
                    Phone = doctor.Contact.Phone,
                    Email = doctor.Contact.Email,
                } : null,
                Specializations = doctor.DoctorSpecializations
                    .Select(ds => ds.Specialization.Name)
                    .ToList(),
                Qualifications = qualificationResponses.ToList()
            };
        }


        public async Task<UpdatedDoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request)
        {
            logger.LogInformation("Updating doctor with ID: {DoctorId}", id);

            var existingDoctor = await doctorRepository.GetById(id);

            if (existingDoctor == null)
            {
                logger.LogWarning("Doctor not found for update with ID: {DoctorId}", id);
                return null;
            }

            var oldLicense = existingDoctor.LicenseNumber;
            var oldExperience = existingDoctor.YearsOfExperience;

            existingDoctor.FirstName = request.FirstName;
            existingDoctor.LastName = request.LastName;
            existingDoctor.DateOfBirth = request.DateOfBirth;
            existingDoctor.Gender = request.Gender;
            existingDoctor.LicenseNumber = request.LicenseNumber;
            existingDoctor.YearsOfExperience = request.YearsOfExperience;

            var specializationDtos = await specializationService.GetAllSpecializationsAsync();

            var validSpecializationIds = request.SpecializationIds
                .Where(id => specializationDtos.Any(dto => dto.Id == id))
                .ToList();

            if (validSpecializationIds.Count != request.SpecializationIds.Count())
            {
                logger.LogWarning("Invalid specialization IDs provided for doctor {DoctorId}. Valid: {ValidCount}, Requested: {RequestedCount}",
                    id, validSpecializationIds.Count, request.SpecializationIds.Count());
                throw new ArgumentException("One or more specialization IDs are invalid.");
            }

            await doctorRepository.UpdateDoctorSpecializationsAsync(id, validSpecializationIds);

            logger.LogInformation("Doctor {DoctorId} updated successfully - {FirstName} {LastName}, License: {OldLicense} → {NewLicense}, Experience: {OldExp} → {NewExp} years",
                id, existingDoctor.FirstName, existingDoctor.LastName, oldLicense, existingDoctor.LicenseNumber, oldExperience, existingDoctor.YearsOfExperience);

            return new UpdatedDoctorDto
            {
                Id = existingDoctor.Id,
                FirstName = existingDoctor.FirstName,
                LastName = existingDoctor.LastName,
                DateOfBirth = existingDoctor.DateOfBirth,
                Gender = existingDoctor.Gender,
                LicenseNumber = existingDoctor.LicenseNumber,
                YearsOfExperience = existingDoctor.YearsOfExperience,
                SpecializationIds = validSpecializationIds,
            };
        }

        public async Task<string?> DeleteDoctorAsync(Guid id)
        {
            logger.LogInformation("Attempting to delete doctor with ID: {DoctorId}", id);

            if (id == Guid.Empty)
            {
                logger.LogWarning("Delete attempt with invalid empty GUID");
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var doctor = await doctorRepository.GetById(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor not found for deletion with ID: {DoctorId}", id);
                return null;
            }

            using var transaction = await doctorRepository.BeginTransactionAsync();

            try
            {
                logger.LogDebug("Deleting doctor {DoctorId} - {FirstName} {LastName}, License: {LicenseNumber}",
                    id, doctor.FirstName, doctor.LastName, doctor.LicenseNumber);

                await doctorRepository.DeleteAsync(id);

                if (doctor.Address != null)
                {
                    await addressService.DeleteAddressAsync(doctor.Address.Id);
                }

                if (doctor.Contact != null)
                {
                    await contactService.DeleteContactAsync(doctor.Contact.Id);
                }

                if (!string.IsNullOrEmpty(doctor.UserId))
                {
                    var identityResult = await userManagementService.DeleteUserByIdAsync(doctor.UserId);
                    if (!identityResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                        logger.LogError("Failed to delete Identity user for doctor {DoctorId}: {Errors}", id, errors);
                        throw new CustomException("Failed to delete associated Identity user: " + errors);
                    }
                }

                await transaction.CommitAsync();

                logger.LogInformation("Doctor {DoctorId} deleted successfully - {FirstName} {LastName}, License: {LicenseNumber}",
                    id, doctor.FirstName, doctor.LastName, doctor.LicenseNumber);

                return "Doctor has been deleted successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Failed to delete doctor with ID: {DoctorId}", id);
                throw new CustomException("An unexpected error occurred while deleting the doctor: " + ex.Message, ex);
            }
        }
    }
}
