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
        private readonly ILogger<DoctorHospitalService> logger;

        public DoctorHospitalService(
            IDoctorHospitalRepository doctorHospitalRepository,
            ILogger<DoctorHospitalService> logger)
        {
            this.doctorHospitalRepository = doctorHospitalRepository;
            this.logger = logger;
        }

        public async Task BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            logger.LogInformation("Attempting to bind Doctor {DoctorId} to Hospital {HospitalId}",
                doctorId, hospitalId);

            var existingBinding = await doctorHospitalRepository.GetBindingAsync(doctorId, hospitalId);
            if (existingBinding != null)
            {
                logger.LogWarning("Binding failed - Doctor {DoctorId} is already bound to Hospital {HospitalId}",
                    doctorId, hospitalId);
                throw new CustomException("Doctor is already bound to this hospital.");
            }

            var doctorHospital = new DoctorHospital
            {
                DoctorId = doctorId,
                HospitalId = hospitalId
            };

            await doctorHospitalRepository.BindAsync(doctorHospital);

            logger.LogInformation("Successfully bound Doctor {DoctorId} to Hospital {HospitalId}",
                doctorId, hospitalId);
        }

        public async Task UnbindDoctorFromHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            logger.LogInformation("Attempting to unbind Doctor {DoctorId} from Hospital {HospitalId}",
                doctorId, hospitalId);

            var existingBinding = await doctorHospitalRepository.GetBindingAsync(doctorId, hospitalId);
            if (existingBinding == null)
            {
                logger.LogWarning("Unbind failed - Doctor {DoctorId} is not bound to Hospital {HospitalId}",
                    doctorId, hospitalId);
                throw new CustomException("Doctor is not bound to this hospital.");
            }

            await doctorHospitalRepository.UnbindAsync(doctorId, hospitalId);

            logger.LogInformation("Successfully unbound Doctor {DoctorId} from Hospital {HospitalId}",
                doctorId, hospitalId);
        }

        public async Task<IEnumerable<DoctorHospitalDto>> GetDoctorsByHospitalAsync(Guid hospitalId)
        {
            logger.LogInformation("Retrieving all doctors for Hospital {HospitalId}", hospitalId);

            var doctorHospitalBindings = await doctorHospitalRepository.GetDoctorsByHospitalAsync(hospitalId);

            var bindings = doctorHospitalBindings.Select(dh => new DoctorHospitalDto
            {
                DoctorId = dh.DoctorId,
                HospitalId = dh.HospitalId
            }).ToList();

            logger.LogInformation("Found {Count} doctors for Hospital {HospitalId}",
                bindings.Count, hospitalId);

            return bindings;
        }

        public async Task<IEnumerable<DoctorHospitalDto>> GetHospitalsByDoctorAsync(Guid doctorId)
        {
            logger.LogInformation("Retrieving all hospitals for Doctor {DoctorId}", doctorId);

            var doctorHospitalBindings = await doctorHospitalRepository.GetHospitalsByDoctorAsync(doctorId);

            var bindings = doctorHospitalBindings.Select(dh => new DoctorHospitalDto
            {
                DoctorId = dh.DoctorId,
                HospitalId = dh.HospitalId
            }).ToList();

            logger.LogInformation("Found {Count} hospitals for Doctor {DoctorId}",
                bindings.Count, doctorId);

            return bindings;
        }
    }
}
