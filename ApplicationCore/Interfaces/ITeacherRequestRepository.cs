using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ITeacherRequestRepository
    {
        Task<List<TeacherRequest>> All();
        Task<TeacherRequest> GetById(int id);
        Task Add(TeacherRequest teacherRequest);
        void Remove(TeacherRequest teacherRequest);
        Task SaveChanges();
    }
}