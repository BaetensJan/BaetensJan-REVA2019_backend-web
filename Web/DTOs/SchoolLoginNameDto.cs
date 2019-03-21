using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Web.DTOs
{
    public class SchoolLoginNameDto
    {
        [Required]
        [DataType(DataType.Text)]
        public string SchoolLoginName { get; set; }
    }
}