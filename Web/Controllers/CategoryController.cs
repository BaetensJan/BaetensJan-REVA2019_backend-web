using System;
using System.Collections.Generic;
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

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("[action]")]
        public Task<IEnumerable<Category>> Categories()
        {
            return _categoryRepository.All();
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

        [HttpPost("[action]")]
        public async Task<Category> UpdateCategory([FromBody] Category category)
        {
            Console.WriteLine(category?.Id);
            Category c = await _categoryRepository.GetById(category.Id);
            c.Name = category.Name;
            c.Description = category.Description;
            await _categoryRepository.SaveChanges();
            return category;
        }
    }
}