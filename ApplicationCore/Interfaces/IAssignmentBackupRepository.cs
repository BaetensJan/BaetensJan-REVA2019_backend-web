using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAssignmentBackupRepository
    {
        Task<AssignmentBackup> Add(Assignment assignment, string schoolName, string groupName, bool createdExhibitor);
        Task<AssignmentBackup> GetById(int id);

        Task SaveChanges();
    }
}