using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities
{
    public class CategoryExhibitor : IJoinEntity<Category>, IJoinEntity<Exhibitor>
    {
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Exhibitor Exhibitor { get; set; }
        public int ExhibitorId { get; set; }

        Category IJoinEntity<Category>.Navigation
        {
            get => Category;
            set => Category = value;
        }

        Exhibitor IJoinEntity<Exhibitor>.Navigation
        {
            get => Exhibitor;
            set => Exhibitor = value;
        }
    }
}