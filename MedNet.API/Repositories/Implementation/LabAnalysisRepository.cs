using MedNet.API.Data;
using MedNet.API.Models;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class LabAnalysisRepository : ILabAnalysisRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LabAnalysisRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<LabAnalysis> CreateAsync(LabAnalysis labAnalysis)
        {
            await dbContext.LabAnalyses.AddAsync(labAnalysis);
            await dbContext.SaveChangesAsync();

            return labAnalysis;
        }

        public async Task<IEnumerable<LabAnalysis>> GetAllAsync()
        {
            return await dbContext.LabAnalyses.ToListAsync();
        }

        public async Task<LabAnalysis?> GetById(Guid id)
        {
            return await dbContext.LabAnalyses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<LabAnalysis?> UpdateAsync(LabAnalysis labAnalysis)
        {
            var existingAnalysis = await dbContext.LabAnalyses.FirstOrDefaultAsync(x => x.Id == labAnalysis.Id);

            if (existingAnalysis != null)
            {
                dbContext.Entry(existingAnalysis).CurrentValues.SetValues(labAnalysis);
                await dbContext.SaveChangesAsync();
                return existingAnalysis;
            }

            return null;
        }

        public async Task<LabAnalysis?> DeleteAsync(Guid id)
        {
            var existingAnalysis = await dbContext.LabAnalyses.FirstOrDefaultAsync(x => x.Id == id);

            if(existingAnalysis is null)
            {
                return null;
            }

            dbContext.LabAnalyses.Remove(existingAnalysis);
            await dbContext.SaveChangesAsync();
            return existingAnalysis;

        }
    }
}