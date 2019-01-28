using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitorController : Controller
    {
        private readonly ExhibitorManager _exhibitorManager;

        public ExhibitorController(IExhibitorRepository exhibitorRepository, ICategoryRepository categoryRepository,
            ICategoryExhibitorRepository categoryExhibitorRepository, IQuestionRepository questionRepository)
        {
            _exhibitorManager = new ExhibitorManager(exhibitorRepository, categoryRepository, questionRepository);
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Exhibitor>> Exhibitors()
        {
            var exhbs = _exhibitorManager.ExhibitorsLight();
            //.Exhibitors().ToList(); Todo: infinite recursive loop (has catExhibitor with exhibitors that have catExhibs and so on).
            return exhbs;
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Exhibitor>> ExhibitorsLight()
        {
            return _exhibitorManager.ExhibitorsLight();
        }

        [HttpDelete("[action]/{id}")]
        public Task<Exhibitor> RemoveExhibitor([FromRoute] int id)
        {
            return _exhibitorManager.RemoveExhibitor(id);
        }

        [HttpPost("[action]")]
        public async Task<Exhibitor> AddExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            var exhibitor = new Exhibitor
            {
                Name = exhibitordto.Name,
                ExhibitorNumber = exhibitordto.ExhibitorNumber,
                X = exhibitordto.X,
                Y = exhibitordto.Y,
                GroupsAtExhibitor = 0,
                Categories = CreateCategories(exhibitordto.CategoryIds)
            };

            await _exhibitorManager.AddExhibitor(exhibitor);
            return exhibitor;
        }

        [HttpPost("[action]")]
        public Task<Exhibitor> UpdateExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            var exhibitor = new Exhibitor
            {
                Id = exhibitordto.Id,
                Name = exhibitordto.Name,
                ExhibitorNumber = exhibitordto.ExhibitorNumber,
                X = exhibitordto.X,
                Y = exhibitordto.Y,
                GroupsAtExhibitor = 0,
                Categories = CreateCategories(exhibitordto.CategoryIds)
            };

            return _exhibitorManager.UpdateExhibitor(exhibitor);
        }

        private static List<CategoryExhibitor> CreateCategories(IReadOnlyCollection<int> categoryIdList)
        {
            var categoryExhibitorList = new List<CategoryExhibitor>();

            if (categoryIdList != null)
            {
                categoryExhibitorList.AddRange(categoryIdList.Select(id => new CategoryExhibitor() {CategoryId = id}));
            }

            return categoryExhibitorList;
        }
    }
}