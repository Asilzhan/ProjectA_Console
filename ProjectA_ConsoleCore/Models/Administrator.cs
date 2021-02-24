using System;
namespace ProjectA_ConsoleCore.Models
{
    public class Administrator : User
    {

        public Administrator(string name, string lastName, DateTime birthday, string login, int passwordHash) : base(name, lastName,
            birthday, login, passwordHash)
        {
            
        }
    }
}