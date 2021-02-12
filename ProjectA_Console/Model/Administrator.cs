using System;
using System.Xml.Serialization;
using System.IO;

namespace ProjectA_Console.Model
{
    public class Administrator : User
    {
        public Administrator(string id, string name, string lastName, DateTime birthday, string email, string login, string password) : base(id, name, lastName,
            birthday, email, login, password)
        {
            
        }
    }
}