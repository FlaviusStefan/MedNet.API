using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository specializationRepository;

        public SpecializationService(ISpecializationRepository specializationRepository)
        {
            this.specializationRepository = specializationRepository;
        }

        public async Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationRequestDto request)
        {
            var specialization = new Specialization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await specializationRepository.CreateAsync(specialization);

            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            };
        }

        public async Task<IEnumerable<SpecializationDto>> GetAllSpecializationsAsync()
        {
            var specializations = await specializationRepository.GetAllAsync();

            return specializations.Select(specialization => new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            }).ToList();
        }

        public async Task<SpecializationDto?> GetSpecializationByIdAsync(Guid id)
        {
            var specialization = await specializationRepository.GetById(id);
            if (specialization == null)
            {
                return null;
            }

            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            };
        }

        public async Task<SpecializationDto?> UpdateSpecializationAsync(Guid id, UpdateSpecializationRequestDto request)
        {
            var existingSpecialization = await specializationRepository.GetById(id);

            if (existingSpecialization == null) return null;

            existingSpecialization.Name = request.Name;
            existingSpecialization.Description = request.Description;


            var updatedSpecialization = await specializationRepository.UpdateAsync(existingSpecialization);

            if (updatedSpecialization == null) return null;

            return new SpecializationDto
            {
                Id = updatedSpecialization.Id,
                Name = updatedSpecialization.Name,
                Description = updatedSpecialization.Description

            };
        }

        public async Task<SpecializationDto?> DeleteSpecializationAsync(Guid id)
        {
            var specialization = await specializationRepository.DeleteAsync(id);
            if (specialization == null) return null;

            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description

            };
        }

        public async Task<Dictionary<Guid, string>> ValidateSpecializationsAsync(IEnumerable<Guid> specializationIds)
        {
            var allSpecializations = await GetAllSpecializationsAsync();
            var validSpecializations = allSpecializations
                .Where(s => specializationIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s.Name);

            if (validSpecializations.Count != specializationIds.Count())
            {
                throw new ArgumentException("One or more specialization IDs are invalid.");
            }

            return validSpecializations;
        }

    }
}
