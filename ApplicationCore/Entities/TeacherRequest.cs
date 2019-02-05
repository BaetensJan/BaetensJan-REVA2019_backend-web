using System;

namespace ApplicationCore.Entities
{
    public class TeacherRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Email { get; set; } 
        public string SchoolName { get; set; } 
        public string Notes { get; set; } 
        public DateTime CreationDate { get; set; }
               
        public TeacherRequest(string name, string surname, string email, string schoolName, string notes)
        {
            Name = name;
            Surname = surname;
            Email = email;
            SchoolName = schoolName;
            Notes = notes;
            CreationDate = DateTime.Now;
        }
    }   
}