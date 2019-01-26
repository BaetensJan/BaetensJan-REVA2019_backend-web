using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryExhibitorRepository
    {
        Task<List<CategoryExhibitor>> All();
        Task<List<CategoryExhibitor>> GetByCategoryId(int id);
        Task<List<CategoryExhibitor>> GetByExhibitorId(int id);
        Task<CategoryExhibitor> GetByCategoryAndExhibitorId(int catId, int exhId);
        Task<int> SaveChanges();
    }
}