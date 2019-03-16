using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ApplicationCore.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Assignment> Assignments { get; set; }
        [NotMapped] public List<string> Members { get; set; }
        public DateTime CreationDate { get; set; }
        public string ApplicationUserId { get; set; }

        /**
         * Attribute is only used by backend (groupConfiguration) to store members as string
         * as sql can not safe List<string>. When getting this from database, it sets the members attribute.
         */
        public string MembersAsString
        {
            get => string.Join(',', Members /*.Where(s => !string.IsNullOrEmpty(s))*/);
            set => Members = string.IsNullOrWhiteSpace(value) ? new List<string>() : value.Split(',').ToList();
        }

        public Group()
        {
            Assignments = new List<Assignment>();
            CreationDate = DateTime.Now;
        }

        public void AddAssignment(Assignment assignment)
        {
            if (Assignments == null) Assignments = new List<Assignment>();
            Assignments.Add(assignment);
        }

        /* => school eerst mappen
        public void StartTour()
        {
            if (DateTime.Now < School.Start)
            {
                throw new ArgumentException("Het uitvoeren van de opdracht is pas mogelijk vanaf: " + School.Start);
            }            
        }
        */
    }
}