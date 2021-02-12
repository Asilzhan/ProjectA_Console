using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;
using ProjectA_Console.Models;
using ProjectA_Console.Views;
using static System.Console;
namespace ProjectA_Console.Controller
{
    public class Controller
    {
        public Student CurrentStudent { get; set; }
        
        View view = new View();
        Model model = new Model();
        public Student Student;
        public void Main()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.MainMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        if (Authenfication())
                        {
                             Clear();
                             view.ShowHappy(Student.Name);
                             StudentCommand();
                        } else
                            view.ShowError();
                        break;
                    case 2:
                        Clear();
                        Register(); break;
                    case 0:
                        return;
                }
            }
        }

        public bool Authenfication()
        {
            string name = view.ReadString("Логин: ");
            int password = view.ReadPass();
            return model.Authenticated(name, password, out Student);
        }

        public void Register()
        {
            WriteLine("Тіркелу");
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            int passwordHash = view.ReadInt("Пароль: ");
            model.TryAddStudent(name, lastName, birthday, course, login, passwordHash);
            WriteLine("Тіркелу сәтті аяқталды");
        }

        public void StudentCommand()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.StudentMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        ShowProblems();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void ShowProblems()
        {
            Clear();
            view.Print(model.Problems);
            Problem p = model.Problems[view.ReadInt(">> ", maxValue: model.Problems.Count)];

            ProblemMenu(p);
        }

        private void ProblemMenu(Problem problem)
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(problem);
                view.ProblemMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        Submit(problem);
                        break;
                    case 2:
                        view.Print(model.GetAttemptsOfStudent(problem, CurrentStudent));
                        break;
                    case 0:
                        return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
                }
                
            }
        }

        public void Submit(Problem problem)
        {
            string sourcePath = view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе Ctrl+V батырмасы арқылы қойыңыз\n", ConsoleColor.Green);

            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }

            string sourceText = File.ReadAllText(sourcePath);
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = "test.exe";
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, sourceText);
            
            Attempt attempt = model.AddAttemption(CurrentStudent, problem);
            
            if (results.Errors.HasErrors)
            {
                view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
            }
            else
            {
                RunSolution(problem, results.PathToAssembly, ref attempt);
            }
        }

        private void RunSolution(Problem problem, string assembly, ref Attempt attempt)
        {
            Process process = new Process();
            process.StartInfo.FileName = assembly;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.ErrorDialog = false;
            
            process.Start();
            
            StreamWriter stdInputWriter  = process.StandardInput;
            StreamReader stdOutputReader  = process.StandardOutput;

            foreach (var testCase in problem.TestCases)
            {
                stdInputWriter.WriteLine(testCase.Input);
                string res = stdOutputReader.ReadToEnd();
                Verdict verdict;
                if (string.Compare(res.Trim(), testCase.Output, StringComparison.InvariantCultureIgnoreCase)==0)
                {
                    attempt.Verdict = Verdict.Accepted;
                    view.Print("Дұрыс жауап!\n", ConsoleColor.Green);
                    ReadKey();
                }
                else
                {
                    attempt.Verdict = Verdict.Wrong_answer;
                }

                var result = new AttemptionResult(testCase.Input, testCase.Output, res);
                attempt.TestCases.Add(result);
            }
        }
    }
}