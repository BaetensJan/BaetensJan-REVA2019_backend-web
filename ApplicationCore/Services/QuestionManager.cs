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

//        public static List<Question> UnansweredQuestionsOfCategory(int categoryId, IEnumerable<Assignment> assignments,
//            List<Question> potentialQuestions)
//        {
//            foreach (var assignment in assignments)
//            {
//                var assignmentQuestion = assignment.Question;
//                
//                // if the Category has not been chosen yet -> continue.
//                if (assignmentQuestion.CategoryExhibitor.CategoryId != categoryId)
//                {
//                    continue;
//                }
//                
//                // else, remove this already answered Question out of the unansweredQuestions list.
//                var answeredQuestion = potentialQuestions.SingleOrDefault(q => q.Id == assignmentQuestion.Id);
//                if (answeredQuestion != null)
//                {
//                    potentialQuestions.Remove(answeredQuestion);
//                }
//            }
//
//            return potentialQuestions;
//        }
    }
}