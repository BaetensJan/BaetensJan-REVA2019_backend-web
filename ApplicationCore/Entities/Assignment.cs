using System;

namespace ApplicationCore.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Notes { get; set; } // notities
        public string Photo { get; set; }
        public Question Question { get; set; }
        public string Answer { get; set; }
        public bool Submitted { get; set; } // if the assignment was submitted or still in progress.
        public bool Extra { get; set; } // extra round.
        public DateTime CreationDate { get; set; }
        public DateTime SubmissionDate { get; set; }

        /**
         * Returns true if the Assignment is about a, by the Group in the app created, Exhibitor. 
         */
        public bool WithCreatedExhibitor(int createdExhibitorQuestionId)
        {
            return Extra && Question.Id ==  createdExhibitorQuestionId;
        }
        
        public Assignment(Question question, bool isExtraRound)
        {
            Question = question;
            Photo = "";
            Answer = "";
            Submitted = false;
            Extra = isExtraRound;
            Notes = "";
            CreationDate = DateTime.Now;
        }
        
        public Assignment(bool isExtraRound)
        {
            Photo = "";
            Answer = "";
            Submitted = false;
            Extra = isExtraRound;
            Notes = "";
            CreationDate = DateTime.Now;
        }

        /**
         * Temporary used in AssignmentRepository (for Select Mapping because of recursive CategoryExhibitor mistake) 
         */
        public Assignment()
        {
            
        }
    }
}