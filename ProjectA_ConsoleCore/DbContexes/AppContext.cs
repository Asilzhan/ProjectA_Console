using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectA_ConsoleCore.Models;

namespace ProjectA_ConsoleCore.DbContexes
{
    public class AppContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Director> Directors { get; set; }  // Директорларды базада сақтайтын кесте қасиеті
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public AppContext()
        {
            Database.EnsureCreated();
            AddSampleData();
        }

        private void AddSampleData()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Students.AddRange(StudentsSampleData());
            Teachers.Add(new Teacher("Малика", "Абрахманова", DateTime.Parse("05.05.1988"), "malika",
                User.GetHashString("asdfg"))
            {
                MyProblems = AddProblem(),
                Salary = 130000 // Оқытушының жалақысын анықтау
            });
            Administrators.Add(new Administrator("Бахытжан", "Ассилбеков", DateTime.Parse("02.12.1982"), "assilbekov",
                User.GetHashString("kaznpu")));
            Problems.AddRange(AddProblem());
            
            /*------------------------Базаға жаңа директор қосу------------------------*/
            Directors.Add(new Director("Байдаулет", "Урмашев", DateTime.Parse("04.02.1972"), "baiden",
                User.GetHashString("bigboss"))
            {
                DailyOverworkBonus = 1000,
                ProblemCountPerDay = 3
            });
            
            SaveChanges();
        }
        private List<Student> StudentsSampleData()
        {
            return new List<Student>()
            {
                new Student("1", "1", DateTime.Now, 1, "1", User.GetHashString("1")),
                new Student("Алмат", "Ергеш", DateTime.Parse("12.05.2000"), 3, "bigsoft", User.GetHashString("12345")),
                new Student("Асылжан", "Жансейт", DateTime.Parse("11.01.2001"), 3, "asilzhan", User.GetHashString("qwerty")),
            };        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=project_a_db;Trusted_Connection=True;");
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
                    },
                    Created = DateTime.Today    // Есептің қосылған уақытын анықтау
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
                    },
                    Created = DateTime.Today    // Есептің қосылған уақытын анықтау
                },
                
                new Problem()
                {
                    Title = "Дележ яблок - 2",
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
                    },
                    Created = DateTime.Today    // Есептің қосылған уақытын анықтау
                }
            };
        }
    }
}