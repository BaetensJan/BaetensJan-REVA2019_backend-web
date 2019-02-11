using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Services;

namespace ApplicationCore.Entities
{
    public class Exhibitor
    {
        public int Id { get; set; }

        public string Name { get; set; }
        private ICollection<CategoryExhibitor> CategoryExhibitors { get; } = new List<CategoryExhibitor>();
        [NotMapped] public ICollection<Category> Categories { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public int GroupsAtExhibitor { get; set; }
        public int TotalNumberOfVisits { get; set; }

        public string ExhibitorNumber { get; set; } // exhibition number - Standnummer

        public DateTime CreationDate { get; set; }

        public Exhibitor()
        {
            Categories = new JoinCollectionFacade<Category, Exhibitor, CategoryExhibitor>(this, CategoryExhibitors);

            GroupsAtExhibitor = 0;
            TotalNumberOfVisits = 0;
            Name = "";
            CreationDate = DateTime.Now;
        }

//        public Exhibitor(int id, string name, List<Category> categories, double x, double y, int groupsAtExhibitor,
//            string exhibitorNumber)
//        {
//            Id = id;
//            Name = name;
//            //Categories = categories;
//            X = x;
//            Y = y;
//            GroupsAtExhibitor = groupsAtExhibitor;
//            ExhibitorNumber = exhibitorNumber;
//            CreationDate = DateTime.Now;
//        }

        public bool Equals(Exhibitor exhibitor)
        {
            return this.Id.Equals(exhibitor.Id);
        }
    }
}