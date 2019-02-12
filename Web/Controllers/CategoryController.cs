using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
        private readonly IQuestionRepository _questionRepository;
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly IConfiguration _configuration;

        public CategoryController(ICategoryRepository categoryRepository, IGroupRepository groupRepository,
            IQuestionRepository questionRepository, IExhibitorRepository exhibitorRepository,
            IConfiguration configuration)

        {
            _categoryRepository = categoryRepository;
            _groupRepository = groupRepository;
            _questionRepository = questionRepository;
            _exhibitorRepository = exhibitorRepository;
            _configuration = configuration;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Category>> Categories()
        {
            var result = (await _categoryRepository.All()).Select(c =>
            {
                var cat = new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreationDate = c.CreationDate,
                    Photo = c.Photo,
                    Description = c.Description
                };
                foreach (var e in c.Exhibitors)
                {
                    cat.Exhibitors.Add(
                        new Exhibitor
                        {
                            Id = e.Id,
                            Name = e.Name,
                            ExhibitorNumber = e.ExhibitorNumber,
                            X = e.X,
                            Y = e.Y,
                            CreationDate = e.CreationDate,
                            GroupsAtExhibitor = e.GroupsAtExhibitor,
                            TotalNumberOfVisits = e.TotalNumberOfVisits
                        });
                }

                return cat;
            }).ToList();

            return result;
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
         * Parameter exhibitorId: when a Group wants all Categories, of which not all related Questions were answered, 
         * of Exhibitor with id == exhibitorId.
         */
        [HttpGet("[action]/{exhibitorId}")]
        public async Task<IEnumerable<Category>> GetUnpickedCategories(int exhibitorId)
        {
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));
            var assignments = group.Assignments;

            var assignmentsLength = assignments.Count;

            // Check if Group is not doing an Extra Round
            if (assignmentsLength < _configuration.GetValue<int>("AmountOfQuestions"))
            {
                var temp = await _categoryRepository.All();
                var categories =
                    temp.ToList(); // todo can dit in 1 regel? Of breekt await _categoryRepository.All().Result.toList() de async? 

                foreach (var assignment in assignments)
                {
                    var category =
                        categories.SingleOrDefault(c => c.Id == assignment.Question.CategoryExhibitor.CategoryId);
                    if (category != null)
                    {
                        categories.Remove(category);
                    }
                }

                return categories;
            }
            else
            {
                IEnumerable<Category> categories;
                var questions = await _questionRepository.GetAll();

                // check if an exhibitorId was given as parameter (if not, then it is equals to -1)
                if (exhibitorId == -1)
                {
                    categories = await _categoryRepository.All();
                }
                else
                {
                    var exhibitor = await _exhibitorRepository.GetById(exhibitorId);
                    categories = exhibitor.Categories; //.Select(categoryExhibitor => categoryExhibitor.Category);
                    questions = questions.Where(q => q.CategoryExhibitor.ExhibitorId == exhibitorId).ToList();
                }

                var unpickedCategories = new List<Category>();
                var counter = 0;

                foreach (var category in categories)
                {
                    // we only need to check the questions related to the current category
                    var categoryQuestions =
                        questions.Where(q => q.CategoryExhibitor.CategoryId == category.Id).ToList();

                    // loop over every Question related to the current Category of the loop.
                    foreach (var question in categoryQuestions)
                    {
                        // check if all questions of the current Category of the loop have already been answered by this Group.
                        foreach (var assignment in assignments)
                        {
                            if (assignment.Question.Id == question.Id)
                            {
                                counter++;
                            }
                        }
                    }

                    // not all categoryQuestions were answered by the Group already, so we can add this Category to UnpickedCategories.
                    if (counter < categoryQuestions.Count) unpickedCategories.Add(category);
                    counter = 0;
                }

                return unpickedCategories;
            }
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