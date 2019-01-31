using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class Exhibitor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CategoryExhibitor> Categories { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public int GroupsAtExhibitor { get; set; }

        public string ExhibitorNumber { get; set; } // exhibition number - Standnummer

        public DateTime CreationDate { get; set; }

        public Exhibitor()
        {
            GroupsAtExhibitor = 0;
            Name = "";
            Categories = new List<CategoryExhibitor>();
            CreationDate = DateTime.Now;
        }

        public Exhibitor(int id, string name, List<Category> categories, double x, double y, int groupsAtExhibitor,
            string exhibitorNumber)
        {
            Id = id;
            Name = name;
            //Categories = categories;
            X = x;
            Y = y;
            GroupsAtExhibitor = groupsAtExhibitor;
            ExhibitorNumber = exhibitorNumber;
            CreationDate = DateTime.Now;
        }

        public bool Equals(Exhibitor exhibitor)
        {
            return this.Id.Equals(exhibitor.Id);
        }
    }
}