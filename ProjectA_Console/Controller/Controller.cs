using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;
using ProjectA_Console.Models;
using ProjectA_Console.Views;

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
                view.MainMenu();
                cmd = view.ReadInt(">>> ");
                
                switch (cmd)
                {
                    case 1:
                        if (Authenfication())
                        {
                             Clear();
                             view.ShowHappy(Student.Name);
                             // StudentCommand(Student);
                        }else
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
            string name = view.ReadString("Аты: ");
            int password = view.ReadInt("Пароль: ");
            return model.Authenticated(name, password, out Student);
        }
        
        public void Register()
        {
            WriteLine("Тіркелу");
            string name = view.ReadString("Аты: ");
            string lastName  = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            int passwordHash = view.ReadInt("Пароль: ");
            model.TryAddStudent(name, lastName, birthday, course, login, passwordHash);
            WriteLine("Тіркелу сәтті аяқталды");

        public void StudentCommand()
        {
            int cmd;
            while (true)
            {
                view.StudentMenu();
                cmd = view.ReadInt(">>> ");
                
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
            view.Print(model.Problems);
            Problem p = model.Problems[view.ReadInt(maxValue: model.Problems.Count)];
        }

        public void Submit(Problem problem)
        {
            string sourceText = view.ReadString("Жауабыңызды осы жерге Ctrl+V батырмасы арқылы қойыңыз\n", ConsoleColor.Green);
            
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = "test.exe";
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, sourceText);
            
            Attempt attempt = model.AddAttemption(CurrentStudent, problem);
            
            if (results.Errors.HasErrors)
            {
                view.Print("Компиляция барысында қате шықты!", ConsoleColor.Red);
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
                if (testCase.Output == res)
                {
                    attempt.Verdict = Verdict.Accepted;
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