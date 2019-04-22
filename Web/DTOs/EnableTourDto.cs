using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    public class EnableTourDto
    {
        [Required]
        public bool EnableTour { get; set; }
        
    }
}