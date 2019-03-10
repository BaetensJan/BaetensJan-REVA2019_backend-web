using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<School> _schools;

        public SchoolRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _schools = dbContext.Schools;
        }

        public async Task<List<School>> GetAll()
        {
            return await _schools.Include(a => a.Groups).ToListAsync();
        }

        private static School MapSchool(School school)
        {
            var groups = new List<Group>(school.Groups);
            groups.ForEach(g =>
            {
                var assignments = new List<Assignment>(g.Assignments);
                assignments.ForEach(a =>
                {
                    a.Question.CategoryExhibitor.Exhibitor.Categories = null;
                    a.Question.CategoryExhibitor.Category.Exhibitors = null;
                });
                g.Assignments = assignments;
            });

            var sch = new School(school.Name, school.Password)
            {
                Id = school.Id,
                Groups = groups,
            };

            return sch;
        }

        public async Task<School> GetById(int id)
        {
            var school = _schools.Include(s => s.Groups).ThenInclude(g => g.Assignments).ThenInclude(a => a.Question)
                .ThenInclude(q => q.CategoryExhibitor)
                .SingleOrDefaultAsync(c => c.Id == id);
            return await school;
        }

        public async Task<School> GetByIdLight(int id)
        {
            var school = _schools.Include(a => a.Groups).ThenInclude(g => g.Assignments).ThenInclude(a => a.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce => ce.Exhibitor)
                .Include(a => a.Groups).ThenInclude(g => g.Assignments).ThenInclude(a => a.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce => ce.Category)
                .SingleOrDefaultAsync(c => c.Id == id);
            return MapSchool(await school);
        }

        public async Task<School> GetByName(string schoolName)
        {
            var school = _schools.SingleOrDefaultAsync(s => s.Name.ToLower().Equals(schoolName.ToLower()));
            return await school;
        }

        public Task Add(School school)
        {
            return _schools.AddAsync(school);
        }

        public void Remove(School school)
        {
            _schools.Remove(school);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}