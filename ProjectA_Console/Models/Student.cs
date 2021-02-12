using System;
using System.Data.Common;
using System.Security.Policy;

namespace ProjectA_Console.Models
{
    public class Student : User
    {
        public Student(string id, string name, string lastName, DateTime birthday, string email, string specialty,
            int course, string login, string password) : base(id, name, lastName, birthday, email, login, password)
        {
            Specialty = specialty;
            Course = course;
        }

        public string Specialty { get; set; }
        public int Course { get; set; }
        
    }
}

