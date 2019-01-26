using System;
using System.Collections.Generic;
using ApplicationCore.Entities;

namespace Infrastructure
{
    public class Mocks
    {
        public static Category[] Categories =
        {
            new Category(1, "hulpmiddelen ADL", "adl.PNG",
                "Deze categorie gaat over Algemene dagelijkse levensverrichtingen"),
            new Category(2, "hulpmiddelen voor kinderen", "kinderen.PNG", "Deze categorie gaat over kinderen"),
            new Category(3, "loophulpmiddelen en rampen", "loopmiddelen.PNG",
                "Deze categorie gaat over loopmiddelen"),
            new Category(4, "rolstoelen", "rolstoel.PNG", "Deze categorie gaat over rolstoelen"),
            new Category(5, "tilhulpmiddelen", "tilhulpmiddelen.PNG", "Deze categorie gaat over tilhulpmiddelen"),
            new Category(6, "aangepaste kleding", "aangepasteKleding.PNG",
                "Deze  categorie gaat over aangepaste kleding"),
            new Category(7, "rolstoelen sport", "rolstoel_sport.PNG", "Deze categorie gaat over sport rolstoelen"),
            new Category(8, "scooters", "rolstoel_sport.PNG", "Deze categorie gaat over scooters"),
            new Category(9, "fietsen", "fiets.PNG", "Deze categorie gaat over fietsen"),
            new Category(10, "omgevingsbediening, domotica, besturing", "domotica.PNG",
                "Deze categorie gaat over omgevingsbedineing"),
            new Category(11, "aangepaste auto's", "auto.PNG", "Deze categorie gaat over aangepaste auto’s"),
            new Category(12, "huisliften", "huisliften.PNG", "Deze categorie gaat over huisliften"),
            new Category(13, "vakantie, reizen, sport", "reizen_sport.PNG", "Deze categorie gaat over vakantie"),
            new Category(14, "overheidsdiensten", "overheidsdiensten.PNG",
                "Deze categorie gaat over overheidsdiensten"),
            new Category(15, "belangenvereinigingen, zelfhulpgroepen", "zelfhulpgroepen.PNG",
                "Deze categorie gaat over zelfhulpgroepen en belangenverenigingen")
        };

        public static List<Category> GetCategories()
        {
            return new List<Category>(Categories);
        }

        public static List<Exhibitor> GetExhibitors()
        {
            List<Exhibitor> exhibitors = new List<Exhibitor>();
            for (int i = 0; i < 25; i++)
            {
                var name = "Exhibitor" + (i + 1);
                var categories = new List<Category>();

                categories.Add(Categories[getRandomIntBetween0And15()]);
                categories.Add(Categories[getRandomIntBetween0And15()]);
                categories.Add(Categories[getRandomIntBetween0And15()]);
                exhibitors.Add(new Exhibitor(i + 1, name, categories, new Random().NextDouble(),
                    new Random().NextDouble(), 0, ""));
            }

            return exhibitors;
        }

        public static List<Assignment> GetAssignments()
        {
            List<Assignment> assignments = new List<Assignment>();
            var exhibs = GetExhibitors();

            for (int i = 0; i < 8; i++)
            {
                var assignm = new Assignment();
                assignm.Id = (i + 1);
                //assignm.Question = "Vraag " + i + "?";
                assignm.Answer = "";
                assignm.Photo = "elektrische-rolstoel-hoog-laag.jpg";
                assignments.Add(assignm);
            }

            return assignments;
        }

        public static List<Group> GetGroups()
        {
            List<Group> groups = new List<Group>();
            var assignments = GetAssignments();

            for (int i = 0; i < 20; i++)
            {
                var group = new Group();
                group.Id = i + 1;
                group.Assignments = assignments;
                group.Members = new List<string>(new[] {"jan", "piet", "corneel"});
                group.Name = "Groep " + (i + 1);
                groups.Add(group);
            }

            return groups;
        }

        private static int getRandomIntBetween0And15()
        {
            var rnd = new Random();
            return rnd.Next(15);
        }
    }
}