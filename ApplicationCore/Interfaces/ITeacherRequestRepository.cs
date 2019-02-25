using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ITeacherRequestRepository
    {
        Task<List<TeacherRequest>> All();
        Task<TeacherRequest> GetById(int id);
        Task<TeacherRequest> GetByEmail(string email);
        Task<TeacherRequest> GetBySchool(string school);
        Task Add(TeacherRequest teacherRequest);
        void Update(TeacherRequest category);
        void Remove(TeacherRequest teacherRequest);
        Task SaveChanges();
    }
}