using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> All();
        Task<List<Category>> ExhibitorsCategoryComboWithQuestion(int exhibitorId);
        Task<Category> GetById(int id);
        Task Add(Category category);
        void Remove(Category category);
        Task SaveChanges();
    }
}