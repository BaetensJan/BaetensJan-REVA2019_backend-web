using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class QuestionManager
    {
        /**
         * Gets Questions of Category with CategoryId equal to parameter CategoryId,
         * that were not yet answered by the Group (check if assignments contains Assignment with a
         * Question that has a CategoryId equal to parameter categoryId).
         */

        public List<Question> UnansweredQuestionsOfCategory(int categoryId, IEnumerable<Assignment> assignments,
            IEnumerable<Question> questions)
        {
            var questions2 = new List<Question>(questions);

            foreach (var assignment in assignments)
            {
                var questionTemp = assignment.Question;
                if (questionTemp.CategoryExhibitor.CategoryId != categoryId) continue;
                var qstn = questions.SingleOrDefault(q => q.Id == questionTemp.Id);

                if (qstn != null)
                {
                    questions2.Remove(qstn);
                }
            }

            return questions2;
        }
    }
}