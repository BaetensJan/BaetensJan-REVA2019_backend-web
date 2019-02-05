using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Category> _categories;
        private readonly DbSet<Question> _questions;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _categories = dbContext.Categories;
            _questions = dbContext.Questions;
        }

        /**
         * Returns the categories of an Exhibitor with exhibitorId equal to parameter exhibitorId,
         * where the combination / relation of the CategoryExhibitor has a related question.
         */

        public Task<List<Category>> ExhibitorsCategoryComboWithQuestion(int exhibitorId)
        {
            var questionCategories = _questions.Where(q => q.CategoryExhibitor.ExhibitorId == exhibitorId).Select(q =>
                new Category
                {
                    Id = q.CategoryExhibitor.CategoryId,
                    Exhibitors = null,
                    Name = q.CategoryExhibitor.Category.Name,
                    Description = q.CategoryExhibitor.Category.Description,
                    Photo = q.CategoryExhibitor.Category.Photo
                }).ToListAsync();
            return questionCategories;
        }

        /**
         * Light data.
         */
        public async Task<IEnumerable<Category>> GetBasic()
        {
            // Abstractie: zorgt ervoor dat enkel de nodige data opgehaald wordt (geen recursieve loop).
            var categories =
                (await _categories.Include(c => c.Exhibitors).ThenInclude(ce => ce.Exhibitor).ToListAsync()).Select(
                    MapCategory);
            return categories;
        }

        public async Task<IEnumerable<Category>> All()
        {
            return await GetBasic();
            // return _categories.ToList(); //Todo: infinite recursive loop (has catExhibitor with exhibitors that have catExhibs and so on).
        }

        public Task<Category> GetById(int id)
        {
            var cat = _categories.SingleOrDefaultAsync(c => c.Id == id);
            return cat;
        }

        private Category MapCategory(Category category)
        {
//            var categoryExhibitors = category.Exhibitors;
//            categoryExhibitors?.ForEach(ce =>
//            {
//                ce.Exhibitor.Categories = null;
//                ce.Category.Exhibitors = null;
//            });
            var cat = new Category
            {
                Id = category.Id,
                Name = category.Name,
                Exhibitors = category.Exhibitors.Select(ce => new CategoryExhibitor
                {
                    ExhibitorId = ce.ExhibitorId,
                    Exhibitor = new Exhibitor
                    {
                        Id = ce.Exhibitor.Id,
                        Name = ce.Exhibitor.Name,
                        X = ce.Exhibitor.X,
                        Y = ce.Exhibitor.Y,
                        GroupsAtExhibitor = ce.Exhibitor.GroupsAtExhibitor,
                        ExhibitorNumber = ce.Exhibitor.ExhibitorNumber
                    }
                }).ToList(),
                Photo = category.Photo,
                Description = category.Description
            };
            return cat;
        }

        public Task Add(Category category)
        {
            return _categories.AddAsync(category);
        }

        public void Remove(Category category)
        {
            _categories.Remove(category);
        }
        
        public void Update(Category category)
        {
            _categories.Update(category);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}