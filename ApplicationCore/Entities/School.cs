using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
        public string Password { get; private set; }

        public DateTime Start { get; set; }

        // date and time of visit to REVA exposition, starttime for starting the tour in the application in android.
        public DateTime CreationDate { get; set; }

        public School(string name, string password)
        {
            Name = name;
            Password = password;
            Groups = new List<Group>();
            CreationDate = DateTime.Now;
        }

        public School()
        {
            CreationDate = DateTime.Now;
        }
    }
}