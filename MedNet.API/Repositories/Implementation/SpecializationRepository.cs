using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;

namespace MedNet.API.Repositories.Implementation
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SpecializationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Specialization> CreateAsync(Specialization specialization)
        {
            await dbContext.Specializations.AddAsync(specialization);
            await dbContext.SaveChangesAsync();

            return specialization;
        }
    }
}
