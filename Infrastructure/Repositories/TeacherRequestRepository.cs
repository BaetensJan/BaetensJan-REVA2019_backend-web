using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<List<TeacherRequest>> All()
        {
            return await _teacherRequests.ToListAsync();
        }

        public async Task<TeacherRequest> GetById(int id)
        {
            var teacherRequest = await _teacherRequests.SingleOrDefaultAsync(c => c.Id == id);
            return teacherRequest;
        }

        public async Task<TeacherRequest> GetByEmail(string email)
        {
            var teacherRequest = await _teacherRequests.SingleOrDefaultAsync(c => c.Email == email);
            return teacherRequest;
        }

        public async Task<TeacherRequest> GetBySchool(string school)
        {
            var teacherRequest = await _teacherRequests.SingleOrDefaultAsync(c => c.SchoolName == school);
            return teacherRequest;
        }

        public Task Add(TeacherRequest teacherRequest)
        {
            return _teacherRequests.AddAsync(teacherRequest);
        }
        
        public void Update(TeacherRequest teacherRequest)
        {
            _teacherRequests.Update(teacherRequest);
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