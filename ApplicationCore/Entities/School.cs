using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class School
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // Name that will be used to login in app (e.g. hogent or hogent.group1)
        public string LoginName { get; set; }
        public List<Group> Groups { get; set; }
        public string Password { get; private set; }

        public DateTime Start { get; set; }

        // date and time of visit to REVA exposition, starttime for starting the tour in the application in android.
        public DateTime CreationDate { get; set; }

        public School(string name, string password)
        {
            Name = name.Trim();
            Password = password;
            Groups = new List<Group>();
            CreationDate = DateTime.Now;
            LoginName = name.Length < 16 ? name :
                name.Replace(" ", "").Substring(0, 14);
        }

        /**
         * Constructor used in schoolRepo mapping method (workaround for recursive data fetching)
         */
        public School(string password)
        {
            Password = password;
        }
    }
}