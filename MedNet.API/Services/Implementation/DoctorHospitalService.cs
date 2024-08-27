using MedNet.API.Exceptions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class DoctorHospitalService : IDoctorHospitalService
    {
        private readonly IDoctorHospitalRepository doctorHospitalRepository;

        public DoctorHospitalService(IDoctorHospitalRepository doctorHospitalRepository)
        {
            this.doctorHospitalRepository = doctorHospitalRepository;
        }

        public async Task BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            var existingBinding = await doctorHospitalRepository.GetBindingAsync(doctorId, hospitalId);
            if (existingBinding != null)
            {
                throw new CustomException("Doctor is already bound to this hospital.");
            }

            var doctorHospital = new DoctorHospital
            {
                DoctorId = doctorId,
                HospitalId = hospitalId
            };

            await doctorHospitalRepository.BindAsync(doctorHospital);
        }

        public async Task UnbindDoctorFromHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            var existingBinding = await doctorHospitalRepository.GetBindingAsync(doctorId, hospitalId);
            if (existingBinding == null)
            {
                throw new CustomException("Doctor is not bound to this hospital.");
            }

            await doctorHospitalRepository.UnbindAsync(doctorId, hospitalId);
        }

        public async Task<IEnumerable<DoctorHospitalDto>> GetDoctorsByHospitalAsync(Guid hospitalId)
        {
            var doctorHospitalBindings = await doctorHospitalRepository.GetDoctorsByHospitalAsync(hospitalId);
            return doctorHospitalBindings.Select(dh => new DoctorHospitalDto
            {
                DoctorId = dh.DoctorId,
                HospitalId = dh.HospitalId
            });
        }

        public async Task<IEnumerable<DoctorHospitalDto>> GetHospitalsByDoctorAsync(Guid doctorId)
        {
            var doctorHospitalBindings = await doctorHospitalRepository.GetHospitalsByDoctorAsync(doctorId);
            return doctorHospitalBindings.Select(dh => new DoctorHospitalDto
            {
                DoctorId = dh.DoctorId,
                HospitalId = dh.HospitalId
            });
        }
    }
}
