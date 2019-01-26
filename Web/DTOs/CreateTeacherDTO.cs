using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class CreateTeacherViewModel : CreateUserViewModel
    {
        [Required]
        public string SchoolName { get; set; }
    }
}