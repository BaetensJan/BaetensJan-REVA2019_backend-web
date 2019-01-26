using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public List<CategoryExhibitor> Exhibitors { get; set; }

        public Category(int id, string name, string photo, string description)
        {
            Id = id;
            Name = name;
            Photo = photo;
            Description = description;
        }

        public Category()
        {
        }
    }
}