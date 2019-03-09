using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAssignmentBackupRepository
    {
        Task Add(Assignment assignment, string schoolName, string groupName, bool createdExhibitor);
        Task SaveChanges();
    }
}