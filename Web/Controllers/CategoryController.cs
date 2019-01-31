using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGroupRepository _groupRepository;

        public CategoryController(ICategoryRepository categoryRepository, IGroupRepository groupRepository)
        {
            _categoryRepository = categoryRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Category>> Categories()
        {
            return _categoryRepository.All();
        }

        /**
         * Returns the Categories that were not yet picker if isExtraRound (normal tour, with 8 assignments has been finished),
         * or returns the categories of which the exhibitors were not all yet picked.
         */
        [HttpGet("[action]")]
        public async Task<IEnumerable<Category>> GetUnpickedCategories()
        {
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));
            var assignments = group.Assignments;
            var categories = await _categoryRepository.All();
            var unpickedCategories = new List<Category>();

            var counter = 0;
            var found = false;

            // check if extra round (when a group has already finished the max number of assignments in a normal tour)
            if (assignments.Count >= 8) //TODO maxNumberOfRounds uit conf file halen
            {
                foreach (var category in categories)
                {
                    foreach (var categoryExhibitor in category.Exhibitors)
                    {
                        foreach (var assignment in assignments)
                        {
                            if (categoryExhibitor.Exhibitor.Id == assignment.Question.CategoryExhibitor.Exhibitor.Id
                                && category.Id == assignment.Question.CategoryExhibitor.Category.Id) counter++;
                        }
                    }

                    // not all exhibitors of a certain Category were picked by the group (otherwise all Exhibitors of a
                    // certain Category would be found in the submitted assignments  (where the chosen Category AND
                    // Exhibitor would match). 
                    if (counter != category.Exhibitors.Count) unpickedCategories.Add(category);
                    counter = 0; // reset counter
                }
            }
            else
            {
                foreach (var category in categories)
                {
                    foreach (var assignment in assignments)
                    {
                        // current Category was already chosen. Break in order to not add it to unpickedCategories.
                        if (assignment.Question.CategoryExhibitor.Category.Id != category.Id) continue;
                        found = true;
                        break;
                    }

                    // all assignments were checked and none had current Category as chosen Category.
                    if (!found) unpickedCategories.Add(category);
                    found = false;
                }
            }

            return unpickedCategories;
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
                Description = category.Description
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