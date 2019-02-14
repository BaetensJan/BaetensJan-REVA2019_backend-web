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
        /**
         * Returns a list of categories that were not yet picked by the Group (check via assignments).
         */
        public IEnumerable<Category> GetCategories(IEnumerable<Assignment> assignments,
            IEnumerable<Category> categories)
        {
            var cats = new List<Category>(categories);

            foreach (var assignment in assignments)
            {
                var category =
                    categories.SingleOrDefault(c => c.Id == assignment.Question.CategoryExhibitor.CategoryId);
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
        public IEnumerable<Category> GetCategoriesExtraRound(IEnumerable<Assignment> assignments,
            IEnumerable<Category> categories, IEnumerable<Question> questions){
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
}