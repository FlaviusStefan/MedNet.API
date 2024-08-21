// Services/DoctorService.cs
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;

namespace MedNet.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request)
        {
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Specialization = request.Specialization,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };

            await doctorRepository.CreateAsync(doctor);

            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender
            };
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
                Gender = doctor.Gender
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
                Gender = doctor.Gender
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

            var updatedDoctor = await doctorRepository.UpdateAsync(existingDoctor);

            if (updatedDoctor == null) return null;

            return new DoctorDto
            {
                Id = updatedDoctor.Id,
                FirstName = updatedDoctor.FirstName,
                LastName = updatedDoctor.LastName,
                Specialization = updatedDoctor.Specialization,
                DateOfBirth = updatedDoctor.DateOfBirth,
                Gender = updatedDoctor.Gender
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
                Gender = doctor.Gender
            };
        }
    }
}
