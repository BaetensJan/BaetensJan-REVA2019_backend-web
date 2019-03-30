using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Group> _groups;

        public GroupRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            _groups = context.Groups;
        }

        public Task<List<Group>> GetAll()
        {
            var groups = _groups.Include(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce
                    => ce.Exhibitor)
                .Include(g => g.Assignments).ThenInclude(f => f.Question).ThenInclude(q => q.CategoryExhibitor)
                .ThenInclude(ce
                    => ce.Category)
                .ToListAsync();
            //.Select(g => MapGroup(g));
            return groups;
        }

        public Task<List<Group>> GetAllLight()
        {
            var groups = _groups.Include(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce
                    => ce.Exhibitor)
                .Include(g => g.Assignments).ThenInclude(f => f.Question).ThenInclude(q => q.CategoryExhibitor)
                .ThenInclude(ce
                    => ce.Category).Select(g => MapGroup(g)).ToListAsync();
            return groups;
        }

        public Task<Group> GetById(int id)
        {
            var group = _groups.Include(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce
                    => ce.Exhibitor)
                .Include(g => g.Assignments).ThenInclude(f => f.Question).ThenInclude(q => q.CategoryExhibitor)
                .ThenInclude(ce
                    => ce.Category)
                .SingleOrDefaultAsync(c => c.Id == id);

            return group;
        }

        public void Update(Group group)
        {
            _groups.Update(group);
        }

        private static Group MapGroup(Group group)
        {
            var assignments = new List<Assignment>(group.Assignments);
            assignments.ForEach(a =>
            {
                a.Question.CategoryExhibitor.Exhibitor.Categories = null;
                a.Question.CategoryExhibitor.Category.Exhibitors = null;
            });
            var gr = new Group
            {
                Id = group.Id,
                Members = group.Members,
                Name = group.Name,
                Assignments = assignments
            };
            return gr;
        }

        public Task Add(Group group)
        {
            return _groups.AddAsync(group);
        }

        public async Task<Group> AddMember(int id, string member)
        {
            var group = await _groups.FirstOrDefaultAsync(x => x.Id == id);
            if (group == null) return null;
            group.Members.Add(member);
            return group;
        }

        public async Task<Group> RemoveMember(int id, string member)
        {
            var group = await _groups.FirstOrDefaultAsync(x => x.Id == id);
            if (group?.Members == null || group.Members.Count < 2)
            {
                return null;
            }

            group.Members.Remove(member);
            return group;
        }

        public void Remove(Group group)
        {
            _groups.Remove(group);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}