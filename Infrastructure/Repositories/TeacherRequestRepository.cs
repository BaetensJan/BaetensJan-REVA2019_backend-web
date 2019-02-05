using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TeacherRequestRepository : ITeacherRequestRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TeacherRequest> _teacherRequests;

        public TeacherRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _teacherRequests = dbContext.TeacherRequests;
        }

        public Task<List<TeacherRequest>> All()
        {
            return _teacherRequests.ToListAsync();
        }

        public Task<TeacherRequest> GetById(int id)
        {
            var teacherRequest = _teacherRequests.SingleOrDefaultAsync(c => c.Id == id);
            return teacherRequest;
        }

        public Task Add(TeacherRequest teacherRequest)
        {
            return _teacherRequests.AddAsync(teacherRequest);
        }

        public void Remove(TeacherRequest teacherRequest)
        {
            _teacherRequests.Remove(teacherRequest);
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}