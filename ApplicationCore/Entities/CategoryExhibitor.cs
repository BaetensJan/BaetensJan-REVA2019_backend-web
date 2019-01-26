namespace ApplicationCore.Entities
{
    public class CategoryExhibitor
    {
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Exhibitor Exhibitor { get; set; }
        public int ExhibitorId { get; set; }
    }
}