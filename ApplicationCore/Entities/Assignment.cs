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
        public bool Extra { get; set; } // if the assignment was created by the group (extra round).
        public DateTime CreationDate { get; set; }
        public DateTime SubmissionDate { get; set; }

        public Assignment()
        {
            Notes = "";
            CreationDate = DateTime.Now;
        }

        public Assignment(Question question)
        {
            Question = question;
            Photo = "";
            Answer = "";
            Submitted = false;
            Extra = question.Id ==
                    795; //question is extra round and new exhibitor related => this is an extra round assignment
            Notes = "";
            CreationDate = DateTime.Now;
        }
    }
}