using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{
    /**
     * Exhibitor object, created by a group while being in an extra round.
     */
    public class CreatedExhibitorDTO
    {
        [Required] public string Name { get; set; }
        public string BoothNumber { get; set; }
        public int categoryId { get; set; }
    }
}