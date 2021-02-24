using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectA_ConsoleCore.Models;

namespace ProjectA_ConsoleCore.DbContexes
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public AppContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Users.AddRange(AddUser());
            Problems.AddRange(AddProblem()); 
            //
            SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=project_a_db;Trusted_Connection=True;");
        }
        public List<User> AddUser()
        {
            return new List<User>()
            {
                new Student("1", "1", DateTime.Now, 1, "1", User.GetHashString("1")),
                new Student("Алмат", "Ергеш", DateTime.Parse("12.05.2000"), 3, "bigsoft", User.GetHashString("12345")),
                new Student("Асылжан", "Жансейт", DateTime.Parse("11.01.2001"), 3, "asilzhan", User.GetHashString("qwerty")),
                new Teacher("Малика", "Абрахманова", DateTime.Parse("05.05.1988"), "malika", User.GetHashString("asdfg")),
                new Administrator("Бахытжан", "Ассилбеков", DateTime.Parse("02.12.1982"), "assilbekov", User.GetHashString("kaznpu")),
            };
        }

        public List<Problem> AddProblem()
        {
            return new List<Problem>()
            {
                new Problem()
                {
                    Title = "Гипотенуза",
                    Text =
                        @"Дано два числа a и b. Найдите гипотенузу треугольника с заданными катетами.

Входные данные
В двух строках вводятся два числа (числа целые,положительные, не превышают 1000).

Выходные данные
Выведите ответ на задачу.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n4", "5"),
                        new TestCase("8\n6", "10")
                    }
                },
                
                new Problem()
                {
                    Title = "Дележ яблок - 1",
                    Text =
                        @"N школьников делят K яблок поровну, неделящийся остаток остается в корзинке. Сколько яблок достанется каждому школьнику?

Входные данные
Программа получает на вход числа N и K.

Выходные данные
Программа должна вывести искомое количество яблок.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n14", "4"),
                        new TestCase("8\n15", "1"),
                        new TestCase("8\n17", "2")
                    }
                },
                
                new Problem()
                {
                    Title = "Дележ яблок - 1",
                    Text =
                        @"N школьников делят K яблок поровну, неделящийся остаток остается в корзинке. Сколько яблок останется в корзинке?

Входные данные
Программа получает на вход числа N и K.

Выходные данные
Программа должна вывести искомое количество яблок.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n14", "2"),
                        new TestCase("8\n15", "7"),
                        new TestCase("8\n17", "1")
                    }
                }
            };
        }
    }
}