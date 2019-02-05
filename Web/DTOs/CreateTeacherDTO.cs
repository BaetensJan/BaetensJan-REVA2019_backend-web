using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class CreateTeacherDTO
    {
        [Required] public string Email { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        [Required] public string SchoolName { get; set; }
        public string Note { get; set; }
    }
}