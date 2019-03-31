using System;

namespace ApplicationCore.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public CategoryExhibitor CategoryExhibitor { get; set; }
        public DateTime CreationDate { get; set; }
        // number of times this question has been chosen.
        public int Answered { get; set; }
        public Question(string questionText, string answerText)
        {
            QuestionText = questionText;
            Answer = answerText;
            CreationDate = DateTime.Now;
        }

        public Question()
        {
            CreationDate = DateTime.Now;
        }
    }
}