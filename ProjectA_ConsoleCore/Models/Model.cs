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
        public List<User> Users { get; set; }
        public List<Problem> Problems { get; set; }
        public List<Attempt> Attempts { get; set; }

        public Model()
        {
            Attempts = new List<Attempt>();
            AppContext = new AppContext();
        }

        #region Methods
        
        public List<Problem> GetProblemsByTeacherId(int teacherId)
        {
            return Problems.FindAll(problem => problem.Id == teacherId);
        }

        public bool TryAddStudent(string name, string lastName, DateTime born, int course, string login, int passHash)
        {
            if (Users.Exists(s => (s.Name == name && s.LastName == lastName) || s.Login == login))
            {
                return false; // Бұндай атты немесе логинді студент бар болса
            }
            
            Student student = new Student(name, lastName, born, course, login, passHash);
            Users.Add(student);
            return true;
        }

        public Problem AddProblem(string title, string text)
        {
            Problem problem = new Problem() {Title = title, Text = text};
            return problem;
        }
        
        public Teacher AddTeacher(string name, string lastName, DateTime birthday, string login, int passwordHash)
        {
            return new Teacher(name, lastName, birthday, login, passwordHash);
        }

        public Attempt AddAttemption(User user, Problem problem)
        {
            return new Attempt(user, problem);
        }

        public bool Authenticated(string login, int password, out User user)
        {
            user = Users.Find(std => std.Login == login && std.CheckPassword(password));
            return !(user is null);
        }

        public List<Attempt> GetAttemptsOfStudent(Problem problem, Student currentStudent)
        {
            return Attempts.FindAll(attempt => attempt.Problem == problem && attempt.User == currentStudent);
        }
        
        #endregion
    }
}