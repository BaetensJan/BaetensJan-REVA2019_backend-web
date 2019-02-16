using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Entities;

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
            IEnumerable<Question> potentialQuestions)
        {
            var unansweredQuestions = new List<Question>(potentialQuestions);

            foreach (var assignment in assignments)
            {
                var assignmentQuestion = assignment.Question;
                
                // if assignmentQuestion's Category is different of the currently chosen Category -> ignore
                if (assignmentQuestion.CategoryExhibitor.CategoryId != categoryId) continue;
                
                // else, remove this already answered Question out of the unansweredQuestions list.
                var answeredQuestion = potentialQuestions.SingleOrDefault(q => q.Id == assignmentQuestion.Id);
                if (answeredQuestion != null)
                {
                    unansweredQuestions.Remove(answeredQuestion);
                }
            }

            return unansweredQuestions;
        }
    }
}