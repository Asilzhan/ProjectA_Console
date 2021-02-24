using System;

namespace ProjectA_ConsoleCore.Models
{
    public class Teacher : User
    {
        public Teacher(string name, string lastName, DateTime birthday, string login, string passwordHash) : base(
            name, lastName,
            birthday, login, passwordHash)
        {
            Role = Role.Teacher;
        }
    }
}