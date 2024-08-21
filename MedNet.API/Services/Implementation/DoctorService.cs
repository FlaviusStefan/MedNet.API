// Services/DoctorService.cs
using MedNet.API.Data;
using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IContactRepository contactRepository;
        private readonly ApplicationDbContext dbContext;

        public DoctorService(IDoctorRepository doctorRepository, IAddressRepository addressRepository, IContactRepository contactRepository, ApplicationDbContext dbContext)
        {
            this.doctorRepository = doctorRepository;
            this.addressRepository = addressRepository;
            this.contactRepository = contactRepository;
            this.dbContext = dbContext;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create Address
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    Street = request.Address.Street,
                    StreetNr = request.Address.StreetNr,
                    City = request.Address.City,
                    State = request.Address.State,
                    Country = request.Address.Country,
                    PostalCode = request.Address.PostalCode
                };

                await addressRepository.CreateAsync(address);

                // Create Contact
                var contact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Phone = request.Contact.Phone,
                    Email = request.Contact.Email
                };

                await contactRepository.CreateAsync(contact);

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
                    AddressId = address.Id,
                    Address = address,
                    ContactId = contact.Id,
                    Contact = contact
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
                    Address = new AddressDto
                    {
                        Id = address.Id,
                        Street = address.Street,
                        StreetNr = address.StreetNr,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        PostalCode = address.PostalCode
                    },
                    Contact = new ContactDto
                    {
                        Id = contact.Id,
                        Phone = contact.Phone,
                        Email = contact.Email
                    }
                };
            }
            catch (AddressValidationException ex)
            {
                // Rollback transaction
                await transaction.RollbackAsync();
                throw new CustomException("Address validation failed: " + ex.Message, ex);
            }
            catch (ContactValidationException ex)
            {
                // Rollback transaction
                await transaction.RollbackAsync();
                throw new CustomException("Contact validation failed: " + ex.Message, ex);
            }
            catch (DoctorValidationException ex)
            {
                // Rollback transaction
                await transaction.RollbackAsync();
                throw new CustomException("Doctor validation failed: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Rollback transaction for any other exceptions
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

            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber= doctor.LicenseNumber,
                YearsOfExperience= doctor.YearsOfExperience,
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

        public async Task<DoctorDto?> DeleteDoctorAsync(Guid id)
        {
            var doctor = await doctorRepository.DeleteAsync(id);

            if (doctor == null) return null;

            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience
            };
        }
    }
}
