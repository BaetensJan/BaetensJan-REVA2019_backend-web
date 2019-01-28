using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ISchoolRepository
    {
        Task<List<School>> GetAll();
        Task<School> GetById(int schoolId);
        School GetByIdLight(int schoolId);
        Task<School> GetByName(string schoolName);
        Task Add(School school);
        void Remove(School school);
        Task SaveChanges();
    }
}