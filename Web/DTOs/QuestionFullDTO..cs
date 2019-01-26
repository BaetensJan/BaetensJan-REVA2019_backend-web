using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class QuestionFullDTO
    {
        public string QuestionText { get; set; }
        public ExhibitorDTO Exhibitor { get; set; }
    }
}