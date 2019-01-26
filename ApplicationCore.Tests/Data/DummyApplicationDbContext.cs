using System.Collections.Generic;
using ApplicationCore.Entities;

namespace ApplicationCore.Tests.Services.Data
{
    public class DummyApplicationDbContext
    {
        private readonly IList<Assignment> _assignments;
        private readonly IList<Category> _categories;
        private readonly IList<Exhibitor> _exhibitors;
        private readonly IList<Group> _groups;

        public DummyApplicationDbContext()
        {
            /*
                        _assignments = new List<Assignment>();
                        _categories = new List<Category>();
                        _exhibitors = new List<Exhibitor>();
                        _groups = new List<Group>();
                        _categories.Add(new Category()
                        {
                            Description = "Heeft 4 wielen en is geen auto",
                            Id = 1, 
                            Name = "rolstoelen"
                        });
                        _categories.Add(new Category()
                        {
                            Description = "Heeft een elektrosnische versnelling", 
                            Id = 2,
                            Name= "eleketrische rolstoelen"
                        });
                        _categories.Add(new Category(){
                            Description = "Onderstuenen van communicatie van kinderen doormiddel van apps deze toont verschillende afbeeldingen... ",
                            Id = 3,
                            Name = "apps Ondersteunende communicatie"});
                        _exhibitors.Add(new Exhibitor()
                        {
                            Categories = _categories.Where(x => x.Id == 1).ToList(),
                            GroupsAtExhibitor = 200,
                            Id = 1,
                            Name = "Jos Van Het Groene Wout",
                            Question = "Wat is de functie van een rolstoel?",
                            X = 0,
                            Y = 0
                        });
                        _exhibitors.Add(new Exhibitor()
                        {
                            Categories = _categories.Where(x => x.Id == 2).ToList(),
                            GroupsAtExhibitor = 20,
                            Id = 2,
                            Name = "Dennis Deroose",
                            Question = "Wat is het verschil tussen een elektrische rolstoel en een gewone rolstoel?",
                            X = 200,
                            Y = 200
                        });
                        _exhibitors.Add(new Exhibitor()
                        {
                            Categories = _categories.Where(x => x.Id == 3).ToList(),
                            GroupsAtExhibitor = 32,
                            Id = 3,
                            Name = "Jan Baetens",
                            Question = "Zijn deze apps gratis om te gebruiken?",
                            X = 500,
                            Y=550
                        });
                        _assignments.Add(new Assignment()
                        {
                            Answers = "2",
                            Exhibitor = Exhibitors.FirstOrDefault(x => x.Id == 1), 
                            Id = 1,
                            Photo = "img", 
                            Notes = "nota 1",
                            Questions = "Wat is de functie van een rolstoel?"
                                
                        });
                        _assignments.Add(new Assignment()
                        {
                            Answers = "een elektrische rolstoel gaat elektrisch verder, moet dus niet geduwd worden.",
                            Exhibitor = Exhibitors.FirstOrDefault(x => x.Id==2),
                            Id = 2,
                            Notes = "note 2",
                            Photo = "img2",
                            Questions = "Wat is het verschil tss een el rolstoel en een gewone rolstoel?"
                        });
                        _assignments.Add(new Assignment()
                        {
                            Answers = "Ze zijn niet gratis maar ze worde wel gesteunt door subsidies",
                            Exhibitor = Exhibitors.FirstOrDefault(x => x.Id == 3), 
                            Id = 3,
                            Notes = "note 3",
                            Photo = "img 3",
                            Questions = "Zijn deze apps gratis om te gebruiken?"
                        });
                        _groups.Add(new Group()
                        {
                            Id = 1,
                            Name = "klas8b1",
                            FinishedAssignments = Assignments.ToList()
                        });
                        _groups.Add(new Group()
                        {
                            Id = 2,
                            Name = "klasF",
                            FinishedAssignments = Assignments.ToList()
                        });
                        _groups.Add(new Group()
                        {
                            Id = 3,
                            Name = "klasX",
                            FinishedAssignments = Assignments.ToList()
                        });
                        */
        }

        public IEnumerable<Assignment> Assignments => _assignments;
        public IEnumerable<Category> Categories => _categories;
        public IEnumerable<Exhibitor> Exhibitors => _exhibitors;
        public IEnumerable<Group> Groups => _groups;
    }
}