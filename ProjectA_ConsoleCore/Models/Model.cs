using System;
using System.Collections.Generic;
using System.Linq;
using ProjectA_ConsoleCore.DbContexes;
using AppContext = ProjectA_ConsoleCore.DbContexes.AppContext;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public AppContext AppContext { get; set; }
        public List<Problem> Problems => AppContext.Problems.ToList();

        public Model()
        {
            AppContext = new AppContext();
        }

        #region Methods
        
        // public List<Problem> GetProblemsByTeacherId(int teacherId)
        // {
        //     return Problems.FindAll(problem => problem.Id == teacherId);
        // }
        public Problem AddProblem(string title, string text)
        {
            return new Problem() {Title = title, Text = text};
        }
        
        public Teacher AddTeacher(string name, string lastName, DateTime birthday, string login, string passwordHash)
        {
            return new Teacher(name, lastName, birthday, login, passwordHash);
        }

        public Attempt AddAttemption(User user, Problem problem)
        {
            return new Attempt(user, problem);
        }

        public bool Authenticated(string login, string passHash, out User user)
        {
            var t = AppContext.Users.ToList();
            user = t.Find(u => u.Login == login && u.CheckPassword(passHash));
            return user != null;
        }

        public List<Attempt> GetAttemptsOfStudent(Problem problem, Student currentStudent)
        {
            return AppContext.Attempts.AsParallel()
                .Where(attempt => attempt.Problem == problem && attempt.User == currentStudent).ToList();
        }
        
        #endregion

        public bool TryAddStudent(string name, string lastName, DateTime birthday, int course, string login, string passwordHash)
        {
            if (AppContext.Users.Any(u => u.Login == login)) return false;
            Student student = new Student(name, lastName, birthday, course, login, passwordHash);
            AppContext.Users.Add(student);
            AppContext.SaveChangesAsync();
            return true;
        }
    }
}