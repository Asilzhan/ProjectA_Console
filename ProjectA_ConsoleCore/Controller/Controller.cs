using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProjectA_ConsoleCore.Models;
using ProjectA_ConsoleCore.Views;
using static System.Console;

namespace ProjectA_ConsoleCore.Controller
{
    public class Controller
    {
        View view = new View();
        Model model = new Model();
        public User CurrentUser;
        public Student CurrentStudent;

        #region Main

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
                        Clear();
                        view.Print("Жүйеге кіру\n", ConsoleColor.Green);
                        if (Authenfication())
                        {
                            Clear();
                            view.ShowHappy(CurrentUser.Name);
                            switch (CurrentUser.Role)
                            {
                                case Role.Administrator:
                                    AdministratorCommand();
                                    break;
                                case Role.Student:
                                    StudentCommand();
                                    break;
                                case Role.Teacher:
                                    TeacherCommand();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        } 
                        else
                            view.ShowError();
                        ReadKey();
                        break;
                    case 2:
                        Register(); break;
                    case 0:
                        return;
                }
            }
        }
        
        public bool Authenfication()
        {
            string name = view.ReadString("Логин: ");
            string passHash = view.ReadPass();
            return model.Authenticated(name, passHash, out CurrentUser);
        }

        public void Register()
        {
            Clear();
            view.Print("Жүйеге тіркелу\n", ConsoleColor.Green);
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            string passwordHash = view.ReadPass();
            if (!model.TryAddStudent(name, lastName, birthday, course, login, passwordHash))
                view.Print("Жүйеде бұл қолданушы бар!!!", ConsoleColor.Yellow);
            else
                view.Print("Тіркелу сәтті аяқталды!!!", ConsoleColor.Green);
            ReadKey();
        }

        #endregion
        
        private void StudentCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        Search();
                        break;
                    case 2:
                        ShowProblems();
                        break;
                    case 3:
                        ProfileCommand();
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
            Problem p = model.Problems[view.ReadInt(maxValue: model.Problems.Count)];

            StudentProblemCommand(p);
        }

        private void StudentProblemCommand(Problem problem)
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(problem);
                view.StudentProblemMenu();
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
        

        private void ProfileCommand()
        {
            throw new NotImplementedException();
        }

        private void TeacherCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        Search();
                        break;
                    case 2:
                        ShowProblems();
                        break;
                    case 3:
                        TeacherProblemCommand();
                        break;
                    case 0:
                        return;
                }
            }
        }
        
        private void TeacherProblemCommand()
        {
            throw new NotImplementedException();
        }
        

        private void Search()
        {
            throw new NotImplementedException();
        }
        
        #region Administrator

        public void AdministratorCommand()
        {
            int cmd;
            while (true)
            {
                view.AdministratorMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        AddTeacher();
                        break;
                    case 0:
                        return;
                }
            }
        }
        private void AddTeacher()
        {
            Clear();
            WriteLine("Мұғалімді тіркеу");
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            string passwordHash = view.ReadPass();
            model.AddTeacher(name, lastName, birthday, login, passwordHash);
            view.Print("Тіркелу сәтті аяқталды!!!", ConsoleColor.Green);
            ReadKey();
        }

        #endregion

        #region Compiler

        private string GenerateRuntimeConfig()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(
                    stream,
                    new JsonWriterOptions() { Indented = true }
                ))
                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("runtimeOptions");
                    writer.WriteStartObject("framework");
                    writer.WriteString("name", "Microsoft.NETCore.App");
                    writer.WriteString(
                        "version",
                        RuntimeInformation.FrameworkDescription.Replace(".NET Core ", "")
                    );
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public void Submit(Problem problem)
        {
            string sourcePath = view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе тышқанмен сүйреп әкеліңіз\n", ConsoleColor.Green);
            sourcePath = sourcePath.Trim(new[] {'\'', '\"'});
            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(File.ReadAllText(sourcePath)));
            var assemblyPath = "test.exe";
            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var compilation = CSharpCompilation.Create(Path.GetFileName(assemblyPath))
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll"))
                )
                .AddSyntaxTrees(syntaxTree);
            var result = compilation.Emit(assemblyPath);
            
            File.WriteAllText(
                Path.ChangeExtension(assemblyPath, "runtimeconfig.json"),
                GenerateRuntimeConfig()
            );
            
            Attempt attempt = model.AddAttemption(CurrentStudent, problem);
            
            if (!result.Success)
            {
                view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
            }
            else
            {
                RunSolution(problem, assemblyPath, ref attempt);
            }
            // TODO: model.Attempts.Add(attempt);
        }

        private void RunSolution(Problem problem, string assembly, ref Attempt attempt)
        {
            Verdict verdict = Verdict.Wrong_answer;
            foreach (var testCase in problem.TestCases)
            {
                Process process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = assembly;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.ErrorDialog = false;
                process.Start();
            
                StreamWriter stdInputWriter  = process.StandardInput;
                StreamReader stdOutputReader  = process.StandardOutput;
                
                stdInputWriter.WriteLine(testCase.Input.Replace(' ', '\n'));
                string res = stdOutputReader.ReadToEnd();
                
                if (string.Compare(res.Trim(), testCase.Output, StringComparison.InvariantCultureIgnoreCase)==0)
                {
                    verdict = Verdict.Accepted;
                }
                else
                {
                    verdict = Verdict.Wrong_answer;
                    break;
                }

                

                var result = new AttemptionResult(testCase.Input, testCase.Output, res);
                attempt.TestCases.Add(result);
                // process.Kill();
            }
            attempt.Verdict = verdict;
            if(verdict == Verdict.Accepted)
                view.Print("Дұрыс жауап!\n", ConsoleColor.Green);
            else 
                view.Print("Қате жауап!\n", ConsoleColor.Green);
            ReadKey();
        }

        #endregion
    }
}