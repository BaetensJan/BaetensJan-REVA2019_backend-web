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
        private readonly DbSet<School> _schools;

        public GroupRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            _groups = context.Groups;
            _schools = context.Schools;
        }

        /**
         * Gets the group and members.
         */
        public Task<List<Group>> GetBasicsBySchoolId(int schoolId)
        {
            return _schools.Include(s => s.Groups).Where(s => s.Id == schoolId).Select(s => s.Groups)
                .FirstOrDefaultAsync();
        }

        public Task<List<Group>> GetAllBySchoolId(int schoolId)
        {
            var groups = _schools.Include(s => s.Groups).ThenInclude(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor)
                .Where(s => s.Id == schoolId).Select(s => s.Groups).FirstOrDefaultAsync();
            return groups;
        }

        public IEnumerable<Group> GetAllBySchoolIdLight(int schoolId)
        {
            var groups = _schools.Include(s => s.Groups).ThenInclude(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce => ce.Exhibitor)
                .Include(s => s.Groups).ThenInclude(g => g.Assignments).ThenInclude(f => f.Question)
                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce => ce.Category)
                .SingleOrDefault(s => s.Id == schoolId)
                ?.Groups.Select(MapGroup);
            return groups;
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

        private Group MapGroup(Group group)
        {
//            group?.Assignments?.ForEach(a =>
//            {
//                var ce = a.Question?.CategoryExhibitor;
//                if (ce != null)
//                {
//                    ce.Exhibitor.Categories = null;
//                    ce.Category.Exhibitors = null;
//                }
//            });
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
//
//        public Task<List<Group>> GetGroupsByName(string groupname)
//        {
//            var group = _groups.Include(g => g.Assignments).ThenInclude(f => f.Question)
//                .ThenInclude(q => q.CategoryExhibitor).ThenInclude(ce
//                    => ce.Exhibitor)
//                .Include(g => g.Assignments).ThenInclude(f => f.Question).ThenInclude(q => q.CategoryExhibitor)
//                .ThenInclude(ce
//                    => ce.Category)
//                .Where(c => c.Name == groupname);
//            return group;
//        }

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
            if (group == null) return null;
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

        public Task<Group> GetBySchoolIdAndGroupName(int schoolId, string groupName)
        {
            return _schools.Include(s => s.Groups).Where(s => s.Id == schoolId)
                .Select(s => s.Groups.SingleOrDefault(g => g.Name.ToLower().Equals(groupName.ToLower())))
                .SingleOrDefaultAsync();
        }
    }
}