using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
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

        public ExhibitorController(IExhibitorRepository exhibitorRepository)
        {
            _exhibitorRepository = exhibitorRepository;
            _exhibitorManager = new ExhibitorManager(exhibitorRepository);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Exhibitors()
        {
//            var exhbs = _exhibitorManager.ExhibitorsLight();
            return Ok(await _exhibitorRepository.All());
        }

        /**
         * returns exhibitor with name equal to parameter exhibitorname.
         */
        [HttpGet("[action]/{exhibitorName}")]
        [Authorize]
        public async Task<IActionResult> ExhibitorByName(string exhibitorName)
        {
            return Ok(await _exhibitorRepository.GetByName(exhibitorName));
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> ExhibitorsLight()
        {
            return Ok(await _exhibitorManager.ExhibitorsLight());
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateExhibitor([FromRoute] int id, [FromBody] ExhibitorDTO exhibitordto)
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
            return Ok(await _exhibitorManager.UpdateExhibitor(id, exhibitor));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveExhibitor([FromRoute] int id)
        {
            return Ok(await _exhibitorManager.RemoveExhibitor(id));
        }

        [HttpDelete("RemoveExhibitors")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> AddExhibitor([FromBody] ExhibitorDTO exhibitordto)
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

            return Ok(exhibitor);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            var e = await _exhibitorRepository.GetById(exhibitordto.Id);
            e.Id = exhibitordto.Id;
            e.Name = exhibitordto.Name;
            e.ExhibitorNumber = exhibitordto.ExhibitorNumber;
            e.X = exhibitordto.X;
            e.Y = exhibitordto.Y;
            e.GroupsAtExhibitor = 0;
//            var lcatexb = CreateCategories(exhibitordto.CategoryIds);
            var exh = await _exhibitorManager.UpdateExhibitor(e);
            return Ok(exh);
         
        }

        private static IEnumerable<CategoryExhibitor> CreateCategories(IReadOnlyCollection<int> categoryIdList)
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