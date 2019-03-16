using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class GroupUpdateDTO
    {
        [Required] public string Name { get; set; }
        [Required] public int GroupId { get; set; }
        [Required] public int SchoolId { get; set; }
        [Required] public bool PasswordChanged { get; set; }
        [Required] public List<string> Members { get; set; }
        public string Password { get; set; }
    }
}