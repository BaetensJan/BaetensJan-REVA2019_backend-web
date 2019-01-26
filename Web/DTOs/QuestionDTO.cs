using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class QuestionDTO
    {
        [Required] public string QuestionText { get; set; }
        [Required] public string AnswerText { get; set; }
        [Required] public int ExhibitorId { get; set; }
        [Required] public int CategoryId { get; set; }
    }
}