using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    /**
     * Exhibitor object, created by a group while being in an extra round.
     */
    public class CreatedExhibitorDto
    {
        [Required] public string Name { get; set; }
        public string BoothNumber { get; set; }
        public int CategoryId { get; set; }
    }
}