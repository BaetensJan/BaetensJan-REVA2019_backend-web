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
        Assignment Add(Assignment assignment);
        Assignment Remove(Assignment assignment);
        Task<int> SaveChanges();
    }
}