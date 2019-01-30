using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExhibitorRepository : IExhibitorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Exhibitor> _exhibitors;

        public ExhibitorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _exhibitors = dbContext.Exhibitors;
        }

        private Exhibitor MapExhibitor(Exhibitor exhibitor)
        {
//            var categoryExhibitors = exhibitor?.Categories;
//            categoryExhibitors?.ForEach(ce =>
//            {
//                ce.Exhibitor.Categories = null;
//                ce.Category.Exhibitors = null;
//            });
            var exh = new Exhibitor
            {
                Id = exhibitor.Id,
                Name = exhibitor.Name,
                X = exhibitor.X,
                Y = exhibitor.Y,
                GroupsAtExhibitor = exhibitor.GroupsAtExhibitor,
                Categories = exhibitor.Categories.Select(ce => new CategoryExhibitor
                {
                    CategoryId = ce.CategoryId,
                    Category = new Category
                    {
                        Id = ce.Category.Id,
                        Name = ce.Category.Name,
                        Photo = ce.Category.Photo,
                        Description = ce.Category.Description
                    }
                }).ToList(),
                ExhibitorNumber = exhibitor.ExhibitorNumber
            };
            return exh;
        }

        /**
         * Light data
         */
        public async Task<IEnumerable<Exhibitor>> AllLight()
        {
            // Abstractie: zorgt ervoor dat enkel de nodige data opgehaald wordt (geen recursieve loop).
            var exhibitors = (await _exhibitors.Include(c=>c.Categories).ThenInclude(ce=>ce.Category).ToListAsync()).Select(MapExhibitor);
            return exhibitors;
        }

        public Task<List<Exhibitor>> All()
        {
            var exhibitors = _exhibitors.Include(e => e.Categories).ThenInclude(c => c.Category).ToListAsync();
//            .Select(MapExhibitor);
            return exhibitors;
        }

        public Task<Exhibitor> GetById(int id)
        {
            var exhibitor = _exhibitors.Include(e => e.Categories).ThenInclude(ce => ce.Category)
                .SingleOrDefaultAsync(c => c.Id == id);
            return exhibitor;
        }

//        public IEnumerable<Exhibitor> GetByCategory(Category category)
//        {
//            return _exhibitors.Where(e => e.Category == category); // of op categoryId checken
//        }

        public Task Add(Exhibitor Exhibitor)
        {
            return _exhibitors.AddAsync(Exhibitor);
        }

        public void Remove(Exhibitor Exhibitor)
        {
            _exhibitors.Remove(Exhibitor);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}