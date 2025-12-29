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


        public DoctorService(
            IDoctorRepository doctorRepository,
            IAddressService addressService,
            IContactService contactService,
            ISpecializationService specializationService,
            IQualificationService qualificationService,
            IUserManagementService userManagementService
            )
        {
            this.doctorRepository = doctorRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.specializationService = specializationService;
            this.qualificationService = qualificationService;
            this.userManagementService = userManagementService;

        }

        public async Task<CreatedDoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            using var transaction = await doctorRepository.BeginTransactionAsync();

            try
            {
                var validSpecializations = await specializationService.ValidateSpecializationsAsync(request.SpecializationIds);

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
                await transaction.CommitAsync();

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
                    Specializations = validSpecializations.Values.ToList()
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred: " + ex.Message, ex);
            }
        }


        public async Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();

            return doctors.Select(doctor => new DoctorResponseDto
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
            });
        }

        public async Task<DoctorResponseDto?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await doctorRepository.GetById(id);

            if (doctor == null) return null;

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
            var existingDoctor = await doctorRepository.GetById(id);

            if (existingDoctor == null) return null;

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
                throw new ArgumentException("One or more specialization IDs are invalid.");
            }
            
            await doctorRepository.UpdateDoctorSpecializationsAsync(id, validSpecializationIds);

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
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var doctor = await doctorRepository.GetById(id);
            if (doctor == null)
            {
                return null;
            }

            using var transaction = await doctorRepository.BeginTransactionAsync();

            try
            {
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
                        throw new CustomException("Failed to delete associated Identity user: " +
                            string.Join(", ", identityResult.Errors.Select(e => e.Description)));
                    }
                }

                await transaction.CommitAsync();

                return "Doctor has been deleted successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred while deleting the doctor: " + ex.Message, ex);
            }
        }
    }
}
