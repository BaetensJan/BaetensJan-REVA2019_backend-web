using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class LoginGroupDTO
    {
        [Required]
        public string SchoolLoginName { get; set; }
        [Required]
        public string GroupName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}