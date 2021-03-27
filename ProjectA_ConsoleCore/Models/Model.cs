using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AppContext = ProjectA_ConsoleCore.DbContexes.AppContext;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public AppContext AppContext { get; set; }
        // public List<Problem> Problems => AppContext.Problems.Include(problem => problem.TestCases).ToList();

        public List<Problem> Problems => AppContext.Teachers.SelectMany(teacher => teacher.MyProblems).Include(problem => problem.TestCases).ToList();
        public List<User> Users => AppContext.Students.ToList() // Студенттерді қосамыз
            .Concat<User>(AppContext.Teachers.ToList())         // Оқытушыларды қосамыз
            .Concat(AppContext.Administrators.ToList())         // Администраторларды қосамыз 
            .Concat(AppContext.Directors.ToList()).ToList();    // Директорларды қосамыз
        public Model()
        {
            AppContext = new AppContext();
        }

        #region Methods

        public void AddTeacher(string name, string lastName, DateTime birthday, string login, string passwordHash)
        {
            AppContext.Teachers.Add(new Teacher(name, lastName, birthday, login, passwordHash));
            AppContext.SaveChanges();
        }

        public Attempt AddAttemption(User user, Problem problem)
        {
            var t = new Attempt(user, problem);
            user.Attempts.Add(t);
            AppContext.Update(user);
            AppContext.Update(t);
            return t;
        }

        public void AddProblem(Teacher teacher, Problem problem)
        {
            teacher.MyProblems.Add(problem);
            AppContext.Update(teacher);
        }
        public bool Authenticated(string login, string passHash, out User user)
        {
            /*----------Аутентификация логикасы жақсартылды----------*/
            return (user = Users.Find(u => u.Login == login && u.CheckPassword(passHash))) != null;
        }

        public List<Attempt> GetAttemptsOfStudent(Problem problem, User currentUser)
        {
            return AppContext.Attempts.AsParallel()
                .Where(attempt => attempt.Problem == problem && attempt.User == currentUser).ToList();
        }
        
        #endregion

        public bool TryAddStudent(string name, string lastName, DateTime birthday, int course, string login, string passwordHash)
        {
            if (AppContext.Students.Any(u => u.Login == login)) return false;
            Student student = new Student(name, lastName, birthday, course, login, passwordHash);
            AppContext.Students.Add(student);
            AppContext.SaveChangesAsync();
            return true;
        }

        /*----Оқытушыны өшіруге арналған RemoveUser методы асыра жүктелді----*/
        public bool RemoveUser(User user)
        {
            if (user == null) return false;
            if (user.Role == Role.Student)
            {
                AppContext.Students.Remove(user as Student);
            } else if (user.Role == Role.Teacher)
            {
                AppContext.Teachers.Remove(user as Teacher);
            } else if (user.Role == Role.Director)
            {
                AppContext.Directors.Remove(user as Director);
            }
            AppContext.SaveChanges();
            return true;
        }

        /*-------Жалақыны өзгертуге арналған метод қосылды-------*/
        public void ChangeSalary(Teacher teacher, double newSalary)
        {
            teacher.Salary = newSalary;
            AppContext.SaveChangesAsync();
        }
    }
}