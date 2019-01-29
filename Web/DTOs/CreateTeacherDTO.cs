using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class CreateTeacherDTO : CreateUserDTO
    {
        [Required]
        public string SchoolName { get; set; }
    }
}