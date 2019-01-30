using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryExhibitorRepository : ICategoryExhibitorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<CategoryExhibitor> _categoryExhibitors;

        public CategoryExhibitorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _categoryExhibitors = dbContext.CategoryExhibitors;
        }

        public Task<List<CategoryExhibitor>> All()
        {
            return _categoryExhibitors.Include(c => c.Category).Include(e => e.Exhibitor).ToListAsync();
        }

        public Task<List<CategoryExhibitor>> GetByCategoryId(int id)
        {
            return _categoryExhibitors.Include(c => c.Category).Include(e => e.Exhibitor)
                .Where(ce => ce.CategoryId == id).ToListAsync();
        }

        public Task<List<CategoryExhibitor>> GetByExhibitorId(int id)
        {
            return _categoryExhibitors.Include(c => c.Category).Include(e => e.Exhibitor)
                .Where(ce => ce.ExhibitorId == id).ToListAsync();
        }

        public Task<CategoryExhibitor> GetByCategoryAndExhibitorId(int catId, int exhId)
        {
            return _categoryExhibitors.Include(c => c.Category).Include(e => e.Exhibitor).SingleOrDefaultAsync(cE => cE.CategoryId == catId && cE.ExhibitorId == exhId);
        }

        public Task<int> SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}