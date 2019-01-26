using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class GroupUpdateDTO
    {
        [Required] public string Name { get; set; }
        [Required] public int GroupId { get; set; }
        public List<string> Members { get; set; }
        public List<Assignment> Assignments { get; set; }
        public string Password { get; set; }

        public GroupUpdateDTO(Group group)
        {
        }
    }
}