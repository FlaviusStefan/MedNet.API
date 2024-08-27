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

        public DoctorService(
            IDoctorRepository doctorRepository,
            IAddressService addressService,
            IContactService contactService,
            ISpecializationService specializationService,
            IQualificationService qualificationService)
        {
            this.doctorRepository = doctorRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.specializationService = specializationService;
            this.qualificationService = qualificationService;
        }

        public async Task<CreatedDoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            using var transaction = await doctorRepository.BeginTransactionAsync();

            try
            {
                if (request.SpecializationIds == null || !request.SpecializationIds.Any())
                {
                    throw new ArgumentException("At least one specialization ID is required.");
                }

                var specializationDtos = await specializationService.GetAllSpecializationsAsync();

                var validSpecializationIds = request.SpecializationIds
                    .Where(id => specializationDtos.Any(dto => dto.Id == id))
                    .ToList();

                if (!validSpecializationIds.Any())
                {
                    throw new ArgumentException("None of the provided specialization IDs are valid.");
                }

                var addressDto = await addressService.CreateAddressAsync(request.Address);

                var contactDto = await contactService.CreateContactAsync(request.Contact);

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    LicenseNumber = request.LicenseNumber,
                    YearsOfExperience = request.YearsOfExperience,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id,
                    DoctorSpecializations = validSpecializationIds.Select(sid => new DoctorSpecialization
                    {
                        DoctorId = Guid.NewGuid(),
                        SpecializationId = sid
                    }).ToList()
                };

                await doctorRepository.CreateAsync(doctor);

                // Commit transaction
                await transaction.CommitAsync();

                var specializationNames = specializationDtos
                    .Where(dto => validSpecializationIds.Contains(dto.Id))
                    .Select(dto => dto.Name)
                    .ToList();


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
                    Specializations = specializationNames
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();

            var specializationDtos = await specializationService.GetAllSpecializationsAsync();
            var qualificationDtos = await qualificationService.GetAllQualificationsAsync();
            var addressDtos = await addressService.GetAllAddressesAsync();
            var contactDtos = await contactService.GetAllContactsAsync();

            return doctors.Select(doctor => new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = addressDtos.FirstOrDefault(a => a.Id == doctor.AddressId),
                Contact = contactDtos.FirstOrDefault(c => c.Id == doctor.ContactId),
                Specializations = specializationDtos
                    .Where(dto => doctor.DoctorSpecializations.Select(ds => ds.SpecializationId).Contains(dto.Id))
                    .Select(dto => dto.Name)
                    .ToList(),
                Qualifications = qualificationDtos
                    .Where(dto => doctor.Qualifications.Select(q => q.Id).Contains(dto.Id))
                    .Select(dto => new QualificationDto
                    {
                        Id = dto.Id,
                        Degree = dto.Degree,
                        Institution = dto.Institution,
                        StudiedYears = dto.StudiedYears,
                        YearOfCompletion = dto.YearOfCompletion
                    }).ToList()
            });
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await doctorRepository.GetById(id);

            if (doctor == null) return null;

            var specializationDtos = await specializationService.GetAllSpecializationsAsync();
            var qualificationDtos = await qualificationService.GetAllQualificationsAsync();
            var addressDto = await addressService.GetAddressByIdAsync(doctor.AddressId);
            var contactDto = await contactService.GetContactByIdAsync(doctor.ContactId);

            return new DoctorDto
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
                Specializations = specializationDtos
                    .Where(dto => doctor.DoctorSpecializations.Select(ds => ds.SpecializationId).Contains(dto.Id))
                    .Select(dto => dto.Name)
                    .ToList(),
                Qualifications = qualificationDtos
                    .Where(dto => doctor.Qualifications.Select(q => q.Id).Contains(dto.Id))
                    .Select(dto => new QualificationDto
                    {
                        Id = dto.Id,
                        Degree = dto.Degree,
                        Institution = dto.Institution,
                        StudiedYears = dto.StudiedYears,
                        YearOfCompletion = dto.YearOfCompletion
                    }).ToList()
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
                if (doctor.Address != null)
                {
                    await addressService.DeleteAddressAsync(doctor.Address.Id);
                }

                if (doctor.Contact != null)
                {
                    await contactService.DeleteContactAsync(doctor.Contact.Id);
                }

                await doctorRepository.DeleteAsync(id);

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
