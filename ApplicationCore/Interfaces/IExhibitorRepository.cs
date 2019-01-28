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
        Task Add(Exhibitor Exhibitor);
        void Remove(Exhibitor Exhibitor);
        Task SaveChanges();
    }
}