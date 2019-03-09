using System;

namespace ApplicationCore.Entities
{
    public class AssignmentBackup
    {
        public int Id { get; set; }
        public string Notes { get; set; } // notities
        public string Photo { get; set; }
        public string QuestionText { get; set; }
        public string GroupName { get; set; }
        public string SchoolName { get; set; }
        public string Answer { get; set; }
        public bool Submitted { get; set; } // if the assignment was submitted or still in progress.
        public bool Extra { get; set; } // extra round.
        public DateTime CreationDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool CreatedExhibitor { get; set; } // if the Exhibitor related to the Question was created by the Group.
    }
}