using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ISchoolRepository
    {
        Task<List<School>> GetAll();
        Task<School> GetById(int schoolId);
        Task<School> GetByIdLight(int schoolId);
        Task<School> GetBySchoolName(string schoolName);
        Task<School> GetBySchoolLoginName(string schoolLoginName);
        Task Add(School school);
        void Remove(School school);
        Task SaveChanges();
    }
}