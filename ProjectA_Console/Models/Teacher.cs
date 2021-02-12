using System;
using System.Collections.Generic;

namespace ProjectA_Console.Models
{
    public class Teacher : User
    {
        public List<Classroom> Classrooms { get; set; }
        public Teacher(string id, string name, string lastName, DateTime birthday, string email, string login, string password) : base(id, name, lastName,
            birthday, email, login, password)
        {
            
        }
    }
}