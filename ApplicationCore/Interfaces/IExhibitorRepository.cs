using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IExhibitorRepository
    {
        Task<List<Exhibitor>> All();
        Task<IEnumerable<Exhibitor>> AllLight();
        Task<Exhibitor> GetById(int id);
        Task<Exhibitor> GetByName(string exhibitorName);
        Task Add(Exhibitor Exhibitor);
        void Update(Exhibitor exhibitor);
        void Remove(Exhibitor Exhibitor);
        void RemoveAllExhibitors(IEnumerable<Exhibitor> exhibitors);
        Task SaveChanges();
    }
}