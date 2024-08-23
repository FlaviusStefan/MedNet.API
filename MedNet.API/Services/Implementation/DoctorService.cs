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
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAddressService _addressService;
        private readonly IContactService _contactService;
        private readonly ISpecializationService specializationService;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IAddressService addressService,
            IContactService contactService,
            ISpecializationService specializationService)
        {
            _doctorRepository = doctorRepository;
            _addressService = addressService;
            _contactService = contactService;
            this.specializationService = specializationService;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            using var transaction = await _doctorRepository.BeginTransactionAsync();

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

                var addressDto = await _addressService.CreateAddressAsync(request.Address);

                var contactDto = await _contactService.CreateContactAsync(request.Contact);

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

                await _doctorRepository.CreateAsync(doctor);

                // Commit transaction
                await transaction.CommitAsync();

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
                    SpecializationIds = validSpecializationIds
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
            var doctors = await _doctorRepository.GetAllAsync();

            return doctors.Select(doctor => new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = doctor.Address != null ? new AddressDto
                {
                    Id = doctor.Address.Id,
                    Street = doctor.Address.Street,
                    StreetNr = doctor.Address.StreetNr,
                    City = doctor.Address.City,
                    State = doctor.Address.State,
                    Country = doctor.Address.Country,
                    PostalCode = doctor.Address.PostalCode
                } : null,
                Contact = doctor.Contact != null ? new ContactDto
                {
                    Id = doctor.Contact.Id,
                    Phone = doctor.Contact.Phone,
                    Email = doctor.Contact.Email
                } : null,
                SpecializationIds = doctor.DoctorSpecializations.Select(ds => ds.SpecializationId).ToList(),
                Specializations = doctor.DoctorSpecializations.Select(ds => ds.Specialization.Name).ToList()
            });
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _doctorRepository.GetById(id);

            if (doctor == null) return null;

            var addressDto = doctor.Address != null ? new AddressDto
            {
                Id = doctor.Address.Id,
                Street = doctor.Address.Street,
                StreetNr = doctor.Address.StreetNr,
                City = doctor.Address.City,
                State = doctor.Address.State,
                Country = doctor.Address.Country,
                PostalCode = doctor.Address.PostalCode
            } : null;

            var contactDto = doctor.Contact != null ? new ContactDto
            {
                Id = doctor.Contact.Id,
                Phone = doctor.Contact.Phone,
                Email = doctor.Contact.Email
            } : null;

            var specializationIds = doctor.DoctorSpecializations
                                  .Select(ds => ds.SpecializationId)
                                  .ToList();

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
                SpecializationIds = specializationIds,
                Specializations = doctor.DoctorSpecializations.Select(ds => ds.Specialization.Name).ToList()
            };
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request)
        {
            var existingDoctor = await _doctorRepository.GetById(id);

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
            }

            await _doctorRepository.UpdateDoctorSpecializationsAsync(id, validSpecializationIds);

            return new DoctorDto
            {
                Id = existingDoctor.Id,
                FirstName = existingDoctor.FirstName,
                LastName = existingDoctor.LastName,
                DateOfBirth = existingDoctor.DateOfBirth,
                Gender = existingDoctor.Gender,
                LicenseNumber = existingDoctor.LicenseNumber,
                YearsOfExperience = existingDoctor.YearsOfExperience,
                SpecializationIds = validSpecializationIds
            };
        }

        public async Task<string?> DeleteDoctorAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid ID", nameof(id));
            }

            var doctor = await _doctorRepository.GetById(id);
            if (doctor == null)
            {
                return null;
            }

            using var transaction = await _doctorRepository.BeginTransactionAsync();

            try
            {
                if (doctor.Address != null)
                {
                    await _addressService.DeleteAddressAsync(doctor.Address.Id);
                }

                if (doctor.Contact != null)
                {
                    await _contactService.DeleteContactAsync(doctor.Contact.Id);
                }

                await _doctorRepository.DeleteAsync(id);

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
