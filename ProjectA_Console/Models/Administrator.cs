using System;

namespace ProjectA_Console.Models
{
    public class Administrator : User
    {
        public Administrator(string id, string name, string lastName, DateTime birthday, string email, string login, string password) : base(id, name, lastName,
            birthday, email, login, password)
        {
            
        }
    }
}