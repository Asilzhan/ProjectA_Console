using System.Collections.Generic;
using System.Linq;
using ProjectA_ConsoleCore.DbContexes;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public UserContext UserContext { get; set; }
        public List<User> Users { get; set; }
        public List<Problem> Problems { get; set; }
        public List<Attempt> Attempts { get; set; }

        public Model()
        {
            Attempts = new List<Attempt>();
            UserContext = new UserContext();
            
        }

        #region Methods
        
        public List<Problem> GetProblemsByTeacherId(int teacherId)
        {
            return Problems.FindAll(problem => problem.Id == teacherId);
        }
        public Problem AddProblem(string title, string text)
        {
            int id = 1;
            if (Problems.Count != 0) id = Problems.Max(s => s.Id) + 1;
            Problem problem = new Problem() {Id = id, Title = title, Text = text};
            return problem;
        }
        public Attempt AddAttemption(User user, Problem problem)
        {
            int id = 1;
            if (Attempts.Count != 0) id = Attempts.Max(a => a.Id) + 1;
            var attempt = new Attempt(id, user, problem);
            return attempt;
        }
        public List<Attempt> GetAttemptsOfStudent(Problem problem, Student currentStudent)
        {
            return Attempts.FindAll(attempt => attempt.Problem == problem && attempt.User == currentStudent);
        }
        
        #endregion
    }
}