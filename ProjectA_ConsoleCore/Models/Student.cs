using System;

namespace ProjectA_ConsoleCore.Models
{
    public class Student : User
    {
        public Student(string name, string lastName, DateTime birthday,
            int course, string login, int passwordHash) : base(name, lastName, birthday, login, passwordHash)
        {
            Course = course;
        }
        public int Course { get; set; }

        public Student()
        {
            
        }
    }
}

