using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IConfiguration _configuration;
        private readonly CategoryManager _categoryManager;

        public CategoryController(ICategoryRepository categoryRepository, IGroupRepository groupRepository,
            IQuestionRepository questionRepository, IExhibitorRepository exhibitorRepository,
            IConfiguration configuration)

        {
            _categoryRepository = categoryRepository;
            _groupRepository = groupRepository;
            _configuration = configuration;
            _categoryManager = new CategoryManager(categoryRepository, exhibitorRepository, questionRepository);
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Category>> Categories()
        {
            return _categoryRepository.All();
        }

        /**
         * returns category with name equal to parameter categoryname.
         */
        [HttpGet("[action]/{categoryName}")]
        public async Task<Category> CategorieByName(string categoryName)
        {
            return await _categoryRepository.GetByName(categoryName);
        }

        /**
         * Returns only the Categories of which not all related Questions were answered by a Group.
         *
         * Parameter exhibitorId: when a Group choose an exhibitor, of which we send the exhibitorId as parameter,
         * we return all Categories, for which for every Category not all related Questions were answered by the Group yet. 
         * 
         */
        [HttpGet("[action]/{exhibitorId}")]
        public async Task<IActionResult> GetUnpickedCategories(int exhibitorId)
        {
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));
            var assignments = group.Assignments;
            
            // Group is doing an extra round if they have submitted more than the amount of questions to be answered
            // in a normal tour.
            var extraRound = assignments.Count >= _configuration.GetValue<int>("AmountOfQuestions");
            return Json(await _categoryManager.GetUnpickedCategories(exhibitorId, assignments, extraRound));
        }

        /**
         * Returns all the categories of a specific exhibitor (with exhibitorId equal to parameter exhibitorId)
         * Where a question exists for the Category - Exhibitor combination.
         */
        [HttpGet("[action]/{exhibitorId}")]
        public Task<List<Category>> CategoryExhibitorComboWithQuestion(int exhibitorId)
        {
            var categories = _categoryRepository.ExhibitorsCategoryComboWithQuestion(exhibitorId);
            return categories;
        }

        [HttpPost("[action]")]
        public async Task<Category> AddCategory([FromBody] Category category)
        {
            Category c = new Category()
            {
                Name = category.Name,
                Description = category.Description,
                Photo = ""
            };

            await _categoryRepository.Add(c);

            await _categoryRepository.SaveChanges();
            return c;
        }

        [HttpDelete("[action]/{id}")]
        public async Task<Category> RemoveCategory([FromRoute] int id)
        {
            Category category = await _categoryRepository.GetById(id);
            _categoryRepository.Remove(category);

            await _categoryRepository.SaveChanges();
            return category;
        }

        [HttpDelete("RemoveCategories")]
        public async Task<ActionResult> RemoveCategories()
        {
            IEnumerable<Category> categories = await _categoryRepository.All();
            if (categories != null)
            {
                _categoryRepository.RemoveAllCategories(categories);
                await _categoryRepository.SaveChanges();
            }

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<Category> UpdateCategory([FromRoute] int id, [FromBody] Category category)
        {
            Category c = await _categoryRepository.GetById(category.Id);
            c.Name = category.Name;
            c.Description = category.Description;
            _categoryRepository.Update(c);

            await _categoryRepository.SaveChanges();
            return category;
        }
    }
}