using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<List<Assignment>> All();
        Task<Assignment> GetById(int id);
        Task<Assignment> GetByIdLight(int id);
        Task Add(Assignment assignment);
        void Remove(Assignment assignment);
        Task SaveChanges();
    }
}