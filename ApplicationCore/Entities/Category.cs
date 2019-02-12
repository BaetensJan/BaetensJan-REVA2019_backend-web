using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Services;

namespace ApplicationCore.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        private ICollection<CategoryExhibitor> CategoryExhibitors { get; } = new List<CategoryExhibitor>();

        [NotMapped] public virtual ICollection<Exhibitor> Exhibitors { get; }

        public DateTime CreationDate { get; set; }

        public Category(int id, string name, string photo, string description)
        {
            Id = id;
            Name = name;
            Photo = photo;
            Description = description;
            CreationDate = DateTime.Now;
            Exhibitors = new JoinCollectionFacade<Exhibitor, Category, CategoryExhibitor>(this, CategoryExhibitors);
        }

        public Category()
        {
            CreationDate = DateTime.Now;
            Exhibitors = new JoinCollectionFacade<Exhibitor, Category, CategoryExhibitor>(this, CategoryExhibitors);
        }
    }
}