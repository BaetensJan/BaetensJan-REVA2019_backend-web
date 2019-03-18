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
        public string Note { get; set; } 
        public DateTime CreationDate { get; set; }
        public bool? Accepted { get; set; }
               
        public TeacherRequest(string name, string surname, string email, string schoolName, string note)
        {
            Name = name;
            Surname = surname;
            Email = email;
            SchoolName = schoolName;
            Note = note;
            CreationDate = DateTime.Now;
        }
    }   
}