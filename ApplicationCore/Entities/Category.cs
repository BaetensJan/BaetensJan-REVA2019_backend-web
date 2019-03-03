using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public List<CategoryExhibitor> Exhibitors { get; set; }
        public DateTime CreationDate { get; set; }

        public Category(int id, string name, string photo, string description)
        {
            Id = id;
            Name = name;
            Photo = photo;
            Description = description;
            CreationDate = DateTime.Now;
        }

        public Category()
        {
            CreationDate = DateTime.Now;
        }
        
        public Category MapCategory()
        {
            var cat = new Category
            {
                Id = Id,
                Name = Name,
                Exhibitors = Exhibitors.Select(ce => new CategoryExhibitor
                {
                    ExhibitorId = ce.ExhibitorId,
                    Exhibitor = new Exhibitor
                    {
                        Id = ce.Exhibitor.Id,
                        Name = ce.Exhibitor.Name,
                        X = ce.Exhibitor.X,
                        Y = ce.Exhibitor.Y,
                        GroupsAtExhibitor = ce.Exhibitor.GroupsAtExhibitor,
                        ExhibitorNumber = ce.Exhibitor.ExhibitorNumber
                    }
                }).ToList(),
                Photo = Photo,
                Description = Description
            };
            return cat;
        }
    }
}