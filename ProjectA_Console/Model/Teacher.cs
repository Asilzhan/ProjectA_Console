using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace ProjectA_Console.Model
{
    public class Teacher : User
    {
        public List<Classroom> Classrooms { get; set; }
        public Teacher(string id, string name, string lastName, DateTime birthday, string email, string password) : base(id, name, lastName,
            birthday, email, password)
        {
            
        }
    }
}