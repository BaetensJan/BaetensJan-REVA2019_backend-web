using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class CategoryManager
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IExhibitorRepository _exhibitorRepo;
        private readonly IQuestionRepository _questionRepo;

        public CategoryManager(ICategoryRepository categoryRepository, IExhibitorRepository exhibitorRepository,
            IQuestionRepository questionRepository)
        {
            _categoryRepo = categoryRepository;
            _exhibitorRepo = exhibitorRepository;
            _questionRepo = questionRepository;
        }

        public async Task<IEnumerable<Category>> GetUnpickedCategories(int exhibitorId,
            IEnumerable<Assignment> assignments, bool extraRound)
        {
            // Check if Group is doing a normal tour (number of assignments done < max number of assignments to do)
            if (!extraRound)
            {
                return GetUnpickedCategoriesNormalTour(assignments, await _categoryRepo.All());
            }

            // Group is doing an Extra Round
            IEnumerable<Category> categories;
            var questions = await _questionRepo.GetAll();

            // check if an exhibitorId was given as parameter (if not, then it is equals to -1)
            if (exhibitorId == -1)
            {
                categories = await _categoryRepo.All();
            }
            else
            {
                var exhibitor = await _exhibitorRepo.GetById(exhibitorId);
                if (exhibitor == null) return null;
                
                // todo, MapCategory() can be removed if recursive problem is fixed.
                categories = exhibitor.Categories.Select(categoryExhibitor => categoryExhibitor.Category.MapCategory());
                    
                // todo, category should know its related questions in db (to not get all questions every time).
                questions = questions.Where(q => q.CategoryExhibitor.ExhibitorId == exhibitorId).ToList();
            }

            return GetUnpickedCategoriesExtraRound(assignments, categories, questions);
        }

        /**
         * Returns a list of categories that were not yet picked by the Group (check via assignments).
         */
        public static IEnumerable<Category> GetUnpickedCategoriesNormalTour(IEnumerable<Assignment> assignments,
            IEnumerable<Category> categories)
        {
            var cats = new List<Category>(categories);

            foreach (var assignment in assignments)
            {
                var category =
                    cats.SingleOrDefault(c => c.Id == assignment.Question.CategoryExhibitor.CategoryId);
                if (category != null)
                {
                    cats.Remove(category);
                }
            }
            return cats;
        }

        /**
         * Returns a list of categories of which not all questions related to that Category are answered by a
         * Group (check via assignments) yet.
         */
        public static IEnumerable<Category> GetUnpickedCategoriesExtraRound(IEnumerable<Assignment> assignments,
            IEnumerable<Category> categories, IEnumerable<Question> questions)
        {
            var unpickedCategories = new List<Category>();
            var counter = 0;

            foreach (var category in categories)
            {
                // we only need to check the questions related to the current category
                var categoryQuestions =
                    new List<Question>(questions.Where(q => q.CategoryExhibitor.CategoryId == category.Id));

                // loop over every Question related to the current Category of the loop.
                foreach (var question in categoryQuestions)
                {
                    // check if all questions of the current Category of the loop have already been answered by this Group.
                    counter += assignments.Count(assignment => assignment.Question.Id == question.Id);
                }

                // not all categoryQuestions were answered by the Group already, so we can add this Category to UnpickedCategories.
                if (counter < categoryQuestions.Count) unpickedCategories.Add(category);
                counter = 0;
            }

            return unpickedCategories;
        }
    }
}