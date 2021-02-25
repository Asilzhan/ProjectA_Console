using System;
namespace ProjectA_ConsoleCore.Models
{
    public class Administrator : User
    {

        public Administrator(string name, string lastName, DateTime birthday, string login, string passwordHash) : base(name, lastName,
            birthday, login, passwordHash)
        {
            Role = Role.Administrator;
        }
    }
}