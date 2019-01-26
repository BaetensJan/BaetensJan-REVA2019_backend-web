using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class GroupDTO
    {
        [Required] public string Name { get; set; }
        public List<string> Members { get; set; }
        public List<Assignment> Assignments { get; set; }
        [Required] public string Password { get; set; }

        public GroupDTO(Group group)
        {
        }
    }
}