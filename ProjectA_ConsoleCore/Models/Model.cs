using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using ProjectA_ConsoleCore.DbContexes;
using AppContext = ProjectA_ConsoleCore.DbContexes.AppContext;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public AppContext AppContext { get; set; }
        // public List<Problem> Problems => AppContext.Problems.Include(problem => problem.TestCases).ToList();

        public List<Problem> Problems => AppContext.Teachers.SelectMany(teacher => teacher.MyProblems)
            .Include(problem => problem.TestCases).ToList();

        public List<User> Users => AppContext.Students.ToList<User>().Concat(AppContext.Teachers.ToList()).ToList();

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
            // AppContext.SaveChanges();
            return t;
        }

        public void AddProblem(Teacher teacher, Problem problem)
        {
            teacher.MyProblems.Add(problem);
            AppContext.Update(teacher);
        }

        public bool Authenticated(string login, string passHash, out User user)
        {
            var students = AppContext.Students.Include(s => s.Attempts).ToList(); //базадан студенттер  және попыткалары туралы ақпартты қосы жүктейді.
            var teachers = AppContext.Teachers.Include(teacher => teacher.MyProblems)
                .ThenInclude(problem => problem.TestCases).ToList();
            var admins = AppContext.Administrators.ToList();

            var t1 = students.Find(u => u.Login == login && u.CheckPassword(passHash));
            if (t1 != null)
            {
                user = t1;
                return true;
            }

            var t2 = teachers.Find(u => u.Login == login && u.CheckPassword(passHash));
            if (t2 != null)
            {
                user = t2;
                return true;
            }

            var t3 = admins.Find(u => u.Login == login && u.CheckPassword(passHash));
            user = t3;
            return t3 != null;
        }

        public List<Attempt> GetAttemptsOfStudent(Problem problem, User currentUser)
        {
            return AppContext.Attempts.AsParallel()
                .Where(attempt => attempt.Problem == problem && attempt.User == currentUser).ToList();
        }

        #endregion

        public bool TryAddStudent(string name, string lastName, DateTime birthday, int course, string login,
            string passwordHash)
        {
            if (AppContext.Students.Any(u => u.Login == login)) return false;
            Student student = new Student(name, lastName, birthday, course, login, passwordHash);
            AppContext.Students.Add(student);
            AppContext.SaveChangesAsync();
            return true;
        }

        public bool RemoveUser(User user)
        {
            if (user == null) return false;
            if (user.Role == Role.Student)
            {
                AppContext.Students.Remove(user as Student);
            }
            else if (user.Role == Role.Teacher)
            {
                AppContext.Teachers.Remove(user as Teacher);
            }

            AppContext.SaveChanges();
            return true;
        }
    }
}