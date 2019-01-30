using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Assignment> _assignments;

        public AssignmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _assignments = dbContext.Assignments;
        }

        public Task<List<Assignment>> All()
        {
            return _assignments.Include(a => a.Question).ThenInclude(exh => exh.CategoryExhibitor)
                .ThenInclude(ce => ce.Exhibitor)
                .Include(a => a.Question).ThenInclude(ae => ae.CategoryExhibitor)
                .ThenInclude(ce => ce.Category)
                .ToListAsync();
        }

        public Task<Assignment> GetById(int id)
        {
            var assignment = _assignments.Include(a => a.Question).ThenInclude(ae => ae.CategoryExhibitor)
                .ThenInclude(ce => ce.Exhibitor)
                .Include(a => a.Question).ThenInclude(ae => ae.CategoryExhibitor).ThenInclude(ce => ce.Category)
                .SingleOrDefaultAsync(c => c.Id == id);
            return assignment;
            // return _assignments.Select(e => AssignmentMap(e)).SingleOrDefaultAsync(c => c.Id == id)?.Result;
        }

        public Task<Assignment> GetByIdLight(int id)
        {
            return _assignments.Select(e => AssignmentMap(e))
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        private Assignment AssignmentMap(Assignment e)
        {
            //Todo question is blijkbaar af en toe null van assignment (al is er wel een questionId in elke assignment object)
            if (e.Question?.CategoryExhibitor?.Exhibitor != null && e.Question?.CategoryExhibitor?.Category != null)
            {
                // Abstractie: zorgt ervoor dat enkel de nodige data opgehaald wordt (geen recursieve loop).
                e.Question = new Question(e.Question.QuestionText, e.Question.Answer)
                {
                    Id = e.Question.Id,
                    CategoryExhibitor = new CategoryExhibitor
                    {
                        CategoryId = e.Question.CategoryExhibitor.CategoryId,
                        Category = new Category
                        {
                            Id = e.Question.CategoryExhibitor.CategoryId,
                            Name = e.Question.CategoryExhibitor.Category.Name,
                            Photo = e.Question.CategoryExhibitor.Category.Photo,
                            Description = e.Question.CategoryExhibitor.Category.Description
                        },
                        ExhibitorId = e.Question.CategoryExhibitor.ExhibitorId,
                        Exhibitor = new Exhibitor
                        {
                            Id = e.Question.CategoryExhibitor.ExhibitorId,
                            Name = e.Question.CategoryExhibitor.Exhibitor.Name,
                            X = e.Question.CategoryExhibitor.Exhibitor.X,
                            Y = e.Question.CategoryExhibitor.Exhibitor.Y,
                            GroupsAtExhibitor = e.Question.CategoryExhibitor.Exhibitor.GroupsAtExhibitor,
                            ExhibitorNumber = e.Question.CategoryExhibitor.Exhibitor.ExhibitorNumber
                            //Categories = e.CategoryExhibitor.Exhibitor.C,
                        }
                    }
                };
            }

            return new Assignment
            {
                Id = e.Id,
                Answer = e.Answer,
                Question = e.Question,
                Notes = e.Notes,
                Photo = e.Photo,
                Submitted = e.Submitted
            };
        }

//        public IEnumerable<Assignment> GetByCategory(Category category)
//        {
//            return _Assignments.Where(e => e.Category == category);
//        }

        public Task Add(Assignment assignment)
        {
            return _assignments.AddAsync(assignment);
        }

        public void Remove(Assignment assignment)
        {
            _assignments.Remove(assignment);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}