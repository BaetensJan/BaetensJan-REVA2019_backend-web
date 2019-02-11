using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class QuestionUpdateDTO : QuestionDTO
    {
        [Required] public int QuestionId { get; set; }
    }
}