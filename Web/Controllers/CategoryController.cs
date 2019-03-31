using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly CategoryManager _categoryManager;
        private readonly GroupManager _groupManager;

        public CategoryController(ICategoryRepository categoryRepository, IGroupRepository groupRepository,
            IQuestionRepository questionRepository, IExhibitorRepository exhibitorRepository,
            IConfiguration configuration)

        {
            _groupManager =
                new GroupManager(configuration,
                    groupRepository); // todo mag via ServiceManager geinjecteerd worden.            
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _categoryManager = new CategoryManager(categoryRepository, exhibitorRepository, questionRepository);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Categories()
        {
            return Ok(await _categoryRepository.All());
        }

        /**
         * returns category with name equal to parameter categoryName.
         */
        [HttpGet("[action]/{categoryName}")]
        [Authorize]
        public async Task<IActionResult> CategoryByName(string categoryName)
        {
            return Ok(await _categoryRepository.GetByName(categoryName));
        }

        /**
         * Returns only the Categories of which not all related Questions were answered by a Group.
         *
         * Parameter exhibitorId: when a Group choose an exhibitor, of which we send the exhibitorId as parameter,
         * we return all Categories, for which for every Category not all related Questions were answered by the Group yet. 
         * 
         */
        [HttpGet("[action]/{exhibitorId}")]
        [Authorize]
        public async Task<IActionResult> GetCategories(int exhibitorId)
        {
            var group = await _groupManager.GetGroup(User.Claims);
            if (group == null)
            {
                return NotFound("groupId not found in token.");
            }

            var assignments = group.Assignments;

            // Group is doing an extra round if they have submitted more than the amount of questions to be answered
            // in a normal tour.
            var extraRound = assignments?.Count >= _configuration.GetValue<int>("AmountOfQuestions");
            return Json(await _categoryManager.GetCategories(exhibitorId, assignments, extraRound));
        }

        /**
         * Returns all the categories of a specific exhibitor (with exhibitorId equal to parameter exhibitorId)
         * Where a question exists for the Category - Exhibitor combination.
         */
        [HttpGet("[action]/{exhibitorId}")]
        [Authorize]
        public async Task<IActionResult> CategoryExhibitorComboWithQuestion(int exhibitorId)
        {
            var categories = await _categoryRepository.ExhibitorsCategoryComboWithQuestion(exhibitorId);
            return Ok(categories);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            var c = new Category
            {
                Name = category.Name,
                Description = category.Description,
                Photo = ""
            };

            await _categoryRepository.Add(c);

            await _categoryRepository.SaveChanges();

            return Ok(c);
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveCategory([FromRoute] int id)
        {
            var category = await _categoryRepository.GetById(id);
            _categoryRepository.Remove(category);

            await _categoryRepository.SaveChanges();
            return Ok(category);
        }

        [HttpDelete("RemoveCategories")]
        [Authorize]
        public async Task<ActionResult> RemoveCategories()
        {
            var categories = await _categoryRepository.All();
            if (categories != null)
            {
                _categoryRepository.RemoveAllCategories(categories);
                await _categoryRepository.SaveChanges();
            }

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] Category category)
        {
            var c = await _categoryRepository.GetById(category.Id);
            c.Name = category.Name;
            c.Description = category.Description;
            _categoryRepository.Update(c);

            await _categoryRepository.SaveChanges();
            return Ok(category);
        }
    }
}