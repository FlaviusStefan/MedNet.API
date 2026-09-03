using AutoMapper;
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
        private readonly IDoctorRepository doctorRepository;
        private readonly IHospitalRepository hospitalRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorHospitalService> logger;

        public DoctorHospitalService(
            IDoctorHospitalRepository doctorHospitalRepository,
            IDoctorRepository doctorRepository,
            IHospitalRepository hospitalRepository,
            ILogger<DoctorHospitalService> logger,
            IMapper mapper)
        {
            this.doctorHospitalRepository = doctorHospitalRepository;
            this.doctorRepository = doctorRepository;
            this.hospitalRepository = hospitalRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            logger.LogInformation("Attempting to bind Doctor {DoctorId} to Hospital {HospitalId}",
                doctorId, hospitalId);

            var doctor = await doctorRepository.GetById(doctorId);
            if (doctor is null)
            {
                logger.LogWarning("Binding failed - Doctor {DoctorId} does not exist", doctorId);
                throw new ArgumentException($"Doctor with ID {doctorId} does not exist.");
            }

            var hospital = await hospitalRepository.GetById(hospitalId);
            if (hospital is null)
            {
                logger.LogWarning("Binding failed - Hospital {HospitalId} does not exist", hospitalId);
                throw new ArgumentException($"Hospital with ID {hospitalId} does not exist.");
            }

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

            var hospital = await hospitalRepository.GetById(hospitalId);
            if (hospital is null)
            {
                logger.LogWarning("Retrieval failed - Hospital {HospitalId} does not exist", hospitalId);
                throw new ArgumentException($"Hospital with ID {hospitalId} does not exist.");
            }

            var doctorHospitalBindings = await doctorHospitalRepository.GetDoctorsByHospitalAsync(hospitalId);
            var bindings = mapper.Map<List<DoctorHospitalDto>>(doctorHospitalBindings);

            logger.LogInformation("Found {Count} doctors for Hospital {HospitalId}",
                bindings.Count, hospitalId);

            return bindings;
        }

        public async Task<IEnumerable<DoctorHospitalDto>> GetHospitalsByDoctorAsync(Guid doctorId)
        {
            logger.LogInformation("Retrieving all hospitals for Doctor {DoctorId}", doctorId);

            var doctor = await doctorRepository.GetById(doctorId);
            if (doctor is null)
            {
                logger.LogWarning("Retrieval failed - Doctor {DoctorId} does not exist", doctorId);
                throw new ArgumentException($"Doctor with ID {doctorId} does not exist.");
            }

            var doctorHospitalBindings = await doctorHospitalRepository.GetHospitalsByDoctorAsync(doctorId);
            var bindings = mapper.Map<List<DoctorHospitalDto>>(doctorHospitalBindings);

            logger.LogInformation("Found {Count} hospitals for Doctor {DoctorId}",
                bindings.Count, doctorId);

            return bindings;
        }
    }
}