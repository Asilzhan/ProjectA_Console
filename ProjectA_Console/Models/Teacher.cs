using System;
using System.Collections.Generic;

namespace ProjectA_Console.Models
{
    public class Teacher : User
    {
        public List<Classroom> Classrooms { get; set; }

        public Teacher(int id, string name, string lastName, DateTime birthday, string login, int passwordHash) : base(id, name, lastName,
            birthday, login, passwordHash)
        {
            
        }
    }
}