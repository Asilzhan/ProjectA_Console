using System;

namespace ProjectA_ConsoleCore.Models
{
    public class Teacher : User
    {
        public Teacher(string name, string lastName, DateTime birthday, string login, int passwordHash) : base(
            name, lastName,
            birthday, login, passwordHash)
        {
        }
    }
}