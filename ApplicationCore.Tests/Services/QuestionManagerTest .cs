using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Entities;
using ApplicationCore.Services;
using Xunit;

namespace ApplicationCore.Tests.Services
{
    public class QuestionManagerTest
    {
        private List<Question> _questions;
        private List<Assignment> _assignments;

        public QuestionManagerTest()
        {
            #region Questions

            _questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    CategoryExhibitor = new CategoryExhibitor
                    {
                        CategoryId = 1
                    }
                },
                new Question
                {
                    Id = 2,
                    CategoryExhibitor = new CategoryExhibitor
                    {
                        CategoryId = 2
                    }
                },
                new Question
                {
                    Id = 3,
                    CategoryExhibitor = new CategoryExhibitor
                    {
                        CategoryId = 3
                    }
                },
                new Question
                {
                    Id = 4,
                    CategoryExhibitor = new CategoryExhibitor
                    {
                        CategoryId = 4
                    }
                }
            };

            #endregion

            #region Assignments

            _assignments = new List<Assignment>
            {
                new Assignment
                {
                    Id = 1,
                    Question = _questions[0]
                },
                new Assignment
                {
                    Id = 2,
                    Question = _questions[1]
                },
                new Assignment
                {
                    Id = 3,
                    Question = _questions[2]
                },
                new Assignment
                {
                    Id = 4,
                    Question = _questions[3]
                }
            };

            #endregion
        }

//        [Theory]
//        [InlineData(1, 99)]
//        public void UnansweredQuestionsOfCategoryTest(int chosenCategoryId, int correctQuestionId)
//        {
//            var questionsOfCategory = _questions.Where(q => q.CategoryExhibitor.CategoryId == chosenCategoryId).ToList();
//            // we add 2 new Questions that haven't been answered by the Group yet.
//            questionsOfCategory.Add(new Question
//            {
//                Id = 99,
//                CategoryExhibitor = new CategoryExhibitor
//                {
//                    CategoryId = 1
//                }
//            });
//            questionsOfCategory.Add(new Question
//            {
//                Id = 100,
//                CategoryExhibitor = new CategoryExhibitor
//                {
//                    CategoryId = 1
//                }
//            });
//
//            var unansweredQuestions = QuestionManager.UnansweredQuestionsOfCategory(chosenCategoryId,
//                _assignments, questionsOfCategory);
//
//            foreach (var question in unansweredQuestions)
//            {
//                Assert.Equal(correctQuestionId, question.Id);
//                correctQuestionId++;
//            }
//        }
    }
}