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
        private readonly IExhibitorRepository _exhibitorRepository;

        public ExhibitorController(IExhibitorRepository exhibitorRepository, ICategoryRepository categoryRepository,
            IQuestionRepository questionRepository)
        {
            _exhibitorRepository = exhibitorRepository;
            _exhibitorManager = new ExhibitorManager(exhibitorRepository, categoryRepository, questionRepository);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Exhibitor>> Exhibitors()
        {
//            var exhbs = _exhibitorManager.ExhibitorsLight();
            return await _exhibitorRepository.All();
        }

        /**
         * returns exhibitor with name equal to parameter exhibitorname.
         */
        [HttpGet("[action]/{exhibitorName}")]
        public async Task<Exhibitor> ExhibitorByName(string exhibitorName)
        {
            return await _exhibitorRepository.GetByName(exhibitorName);
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Exhibitor>> ExhibitorsLight()
        {
            return _exhibitorManager.ExhibitorsLight();
        }

        [HttpPut("[action]/{id}")]
        public Task<Exhibitor> UpdateExhibitor([FromRoute] int id, [FromBody] ExhibitorDTO exhibitordto)
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
            return _exhibitorManager.UpdateExhibitor(id, exhibitor);
        }

        [HttpDelete("[action]/{id}")]
        public Task<Exhibitor> RemoveExhibitor([FromRoute] int id)
        {
            return _exhibitorManager.RemoveExhibitor(id);
        }

        [HttpDelete("RemoveExhibitors")]
        public async Task<ActionResult> RemoveExhibitors()
        {
            IEnumerable<Exhibitor> exhibitors = await _exhibitorRepository.All();
            if (exhibitors != null)
            {
                _exhibitorRepository.RemoveAllExhibitors(exhibitors);
                await _exhibitorRepository.SaveChanges();
            }

            return Ok();
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

            //_exhibitorManager.AddExhibitor(exhibitor);
            await _exhibitorRepository.Add(exhibitor);
            await _exhibitorRepository.SaveChanges();

            return exhibitor;
        }

        [HttpPost("[action]")]
        public async Task<Exhibitor> UpdateExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            Exhibitor e = await _exhibitorRepository.GetById(exhibitordto.Id);
            e.Id = exhibitordto.Id;
            e.Name = exhibitordto.Name;
            e.ExhibitorNumber = exhibitordto.ExhibitorNumber;
            e.X = exhibitordto.X;
            e.Y = exhibitordto.Y;
            e.GroupsAtExhibitor = 0;
            //e.Categories = 
            List<CategoryExhibitor> lcatexb = CreateCategories(exhibitordto.CategoryIds);
            //e.Categories = 
            Exhibitor exh = await _exhibitorManager.UpdateExhibitor(e);
            return exh;
            /*var exhibitor = new Exhibitor
            {
                Id = exhibitordto.Id,
                Name = exhibitordto.Name,
                ExhibitorNumber = exhibitordto.ExhibitorNumber,
                X = exhibitordto.X,
                Y = exhibitordto.Y,
                GroupsAtExhibitor = 0,
                Categories = CreateCategories(exhibitordto.CategoryIds)
            };
            
            return await _exhibitorRepository.SaveChanges();*/
            //return await _exhibitorManager.UpdateExhibitor(exhibitor);
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