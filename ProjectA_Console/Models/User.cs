using System;

namespace ProjectA_Console.Models
{
    [Serializable]
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }

        public string Login { get; set; }

        private readonly int _passwordHash;
        
        public User(string id, string name, string lastName, DateTime birthday, string email, string login, string password)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Birthday = birthday;
            Email = email;
            Login = login;
            _passwordHash = password.GetHashCode();
        }
        
        public User(string password)
        {
            _passwordHash = password.GetHashCode();
        }
        
        public bool CheckPassword(string pass) => _passwordHash.Equals(pass.GetHashCode());
    }
}