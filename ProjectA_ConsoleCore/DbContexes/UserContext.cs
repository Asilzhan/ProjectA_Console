using System;
using Microsoft.EntityFrameworkCore;
using ProjectA_Console.Models;

namespace ProjectA_ConsoleCore.DbContexes
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext()
        {
            Database.EnsureCreated();
            Users.Add(new Student(1, "1", "1", DateTime.Now, 1, "1", "1".GetHashCode()));
            SaveChanges();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=project_a_db;Trusted_Connection=True;");
        }
    }
}