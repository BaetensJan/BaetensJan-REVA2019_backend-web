using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class QuestionUpdateDTO : QuestionDTO
    {
        [Required] public int QuestionId { get; set; }
    }
}