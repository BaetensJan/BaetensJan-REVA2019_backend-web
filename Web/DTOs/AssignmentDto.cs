namespace Web.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }

        public string Answer { get; set; }

        public string Notes { get; set; }

        public string Photo { get; set; }
        
//        public bool Extra { get; set; } // extra round
        
        public bool CreatedExhibitor { get; set; }
        
//        public QuestionFullDTO Question { get; set; }
        
    }
    
}