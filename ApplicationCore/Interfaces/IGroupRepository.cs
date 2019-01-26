using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetBasicsBySchoolId(int schoolId);
        IEnumerable<Group> GetAllBySchoolIdLight(int schoolId);
        Task<List<Group>> GetAll();
        Task<List<Group>> GetAllLight();
        Task<Group> GetById(int groupId);
        Group Add(Group group);
        Group Update(Group group);
        Task<Group> AddMember(int id, string member);
        Task<Group> RemoveMember(int id, string member);
        Group Remove(Group group);
        Task<int> SaveChanges();
        Task<Group> GetBySchoolIdAndGroupName(int schoolId, string groupname);
    }
}