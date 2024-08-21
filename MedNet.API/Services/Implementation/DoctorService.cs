using MedNet.API.Data;
using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace MedNet.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IAddressService addressService;
        private readonly IContactService contactService;
        private readonly ApplicationDbContext dbContext;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IAddressService addressService,
            IContactService contactService,
            ApplicationDbContext dbContext)
        {
            this.doctorRepository = doctorRepository;
            this.addressService = addressService;
            this.contactService = contactService;
            this.dbContext = dbContext;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create Address
                var addressDto = await addressService.CreateAddressAsync(request.Address);

                // Create Contact
                var contactDto = await contactService.CreateContactAsync(request.Contact);

                // Create Doctor
                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Specialization = request.Specialization,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    LicenseNumber = request.LicenseNumber,
                    YearsOfExperience = request.YearsOfExperience,
                    AddressId = addressDto.Id,
                    ContactId = contactDto.Id
                };

                await doctorRepository.CreateAsync(doctor);

                // Commit transaction
                await transaction.CommitAsync();

                return new DoctorDto
                {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Specialization = doctor.Specialization,
                    DateOfBirth = doctor.DateOfBirth,
                    Gender = doctor.Gender,
                    LicenseNumber = doctor.LicenseNumber,
                    YearsOfExperience = doctor.YearsOfExperience,
                    Address = addressDto,
                    Contact = contactDto
                };
            }
            catch (Exception ex)
            {
                // Rollback transaction
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();

            return doctors.Select(doctor => new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
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
                } : null

            });
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await doctorRepository.GetById(id);

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

            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = addressDto,
                Contact = contactDto
            };
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request)
        {
            var existingDoctor = await doctorRepository.GetById(id);

            if (existingDoctor == null) return null;

            existingDoctor.FirstName = request.FirstName;
            existingDoctor.LastName = request.LastName;
            existingDoctor.Specialization = request.Specialization;
            existingDoctor.DateOfBirth = request.DateOfBirth;
            existingDoctor.Gender = request.Gender;
            existingDoctor.LicenseNumber = request.LicenseNumber;
            existingDoctor.YearsOfExperience = request.YearsOfExperience;

            var updatedDoctor = await doctorRepository.UpdateAsync(existingDoctor);

            if (updatedDoctor == null) return null;

            return new DoctorDto
            {
                Id = updatedDoctor.Id,
                FirstName = updatedDoctor.FirstName,
                LastName = updatedDoctor.LastName,
                Specialization = updatedDoctor.Specialization,
                DateOfBirth = updatedDoctor.DateOfBirth,
                Gender = updatedDoctor.Gender,
                LicenseNumber = updatedDoctor.LicenseNumber,
                YearsOfExperience = updatedDoctor.YearsOfExperience
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

            using var transaction = await dbContext.Database.BeginTransactionAsync();

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

                var deletedDoctor = await doctorRepository.DeleteAsync(id);

                await transaction.CommitAsync();

                return "Doctor has been deleted succesfully!";
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CustomException("An unexpected error occurred while deleting the doctor: " + ex.Message, ex);
            }
        }



    }
}
