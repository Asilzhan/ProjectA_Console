using System;

namespace ProjectA_ConsoleCore.Models
{
    public class Director : User
    {
        public Director(string name, string lastName, DateTime birthday, string login, string passwordHash) : base(name, lastName,
            birthday, login, passwordHash)
        {
            Role = Role.Director;
        }

        public double DailyOverworkBonus { get; set; }
        public int ProblemCountPerDay { get; set; }
    }
}