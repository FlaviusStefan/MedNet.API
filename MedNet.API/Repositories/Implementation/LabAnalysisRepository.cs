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
            return labAnalysis;
        }

        public async Task<IEnumerable<LabAnalysis>> GetAllAsync()
        {
            return await dbContext.LabAnalyses
                .AsNoTracking()
                .Include(la => la.LabTests)
                .ToListAsync();
        }

        public async Task<LabAnalysis?> GetById(Guid id)
        {
            return await dbContext.LabAnalyses
                .AsNoTracking()
                .Include(la => la.LabTests)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<LabAnalysis?> UpdateAsync(LabAnalysis labAnalysis)
        {
            var existingAnalysis = await dbContext.LabAnalyses
                .FirstOrDefaultAsync(x => x.Id == labAnalysis.Id);

            if (existingAnalysis is null)
            {
                return null;
            }

            existingAnalysis.AnalysisDate = labAnalysis.AnalysisDate;
            existingAnalysis.AnalysisType = labAnalysis.AnalysisType;

            return existingAnalysis;
        }

        public async Task<LabAnalysis?> DeleteAsync(Guid id)
        {
            var existingAnalysis = await dbContext.LabAnalyses.FirstOrDefaultAsync(x => x.Id == id);

            if(existingAnalysis is null)
            {
                return null;
            }

            dbContext.LabAnalyses.Remove(existingAnalysis);
            return existingAnalysis;

        }
    }
}