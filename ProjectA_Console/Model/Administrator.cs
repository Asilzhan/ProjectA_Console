using System;

namespace ProjectA_Console.Model
{
    public class Administrator : User
    {
        public Administrator(string id, string name, string lastName, DateTime birthday, string email, string password) : base(id, name, lastName,
            birthday, email, password)
        {
            
        }
    }
}