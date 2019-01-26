using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }

        public string Answer { get; set; }

        public string Notes { get; set; }

        public string Photo { get; set; }
        
        public bool Extra { get; set; }
        
        public QuestionFullDTO Question { get; set; }
        
    }
    
}