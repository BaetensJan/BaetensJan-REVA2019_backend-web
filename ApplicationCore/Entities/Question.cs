namespace ApplicationCore.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public CategoryExhibitor CategoryExhibitor { get; set; }
        
           
        public Question(string questionText, string answerText)
        {
            QuestionText = questionText;
            Answer = answerText;
        }
        
        public Question(){}
    }
}