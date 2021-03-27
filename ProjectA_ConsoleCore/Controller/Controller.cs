using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                            // view.ShowHappy(CurrentUser.Name);
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
                                case Role.Director:     // Директорға арнап меню командасы құрылды
                                    DirectorCommand();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        } 
                        else
                        {
                            view.ShowError();
                            ReadKey(); 
                        }
                        break;
                    case 2:
                        Register(); 
                        break;
                    case 0:
                        view.GoodBye();
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
                view.Print("Жүйеде бұл қолданушы бар!!!\n", ConsoleColor.Yellow);
            else
                view.Print("Тіркелу сәтті аяқталды!!!\n", ConsoleColor.Green);
            ReadKey();
        }

        #endregion
        
        private void StudentCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        try
                        {
                            Search();
                        }
                        catch (NotImplementedException notImp)
                        {
                            view.Print("Бұл меню жасалу үстінде!!!\n", ConsoleColor.Green);
                        }
                        break;
                    case 2:
                        SelectProblems();
                        break;
                    case 3:
                        ProfileCommand();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void ProfileCommand()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(CurrentUser);
                view.EditMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        EditPass();
                        break;
                    case 0:
                        return;
                }
            }
        }
        private void EditPass()
        {
            string lastPass = view.ReadPass("Ескі парольіңізді енгізіңіз: ");
            if (lastPass != CurrentUser.PasswordHash)
            {
                view.Println("Пароль қате! Парольді өзгерте алмайсыз");
                return;
            }
            CurrentUser.PasswordHash =  view.ReadPass("Жаңа пароль енгізіңіз: ");
            model.AppContext.Update(CurrentUser);
            model.AppContext.SaveChanges();
            view.Print("Пароль сәтті түрде өзгертілді!!!\n", ConsoleColor.Green);
            view.Wait();
        }
        
        private void SelectProblems(List<Problem> list = null)
        {
            Clear();
            if (list == null)
            {
                list = model.Problems;
            }
            var problem = view.Select(list);
            if(problem == null) return;
            StudentProblemCommand(problem);
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
                        view.Print(CurrentUser.Attempts);
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

        private void TeacherCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        try
                        {
                            Search();
                        }
                        catch (NotImplementedException notImp)
                        {
                            view.Print("Бұл меню жасалу үстінде!!!", ConsoleColor.Green);
                        }
                        break;
                    case 2:
                        TeacherProblemsMenu();
                        break;
                    case 3:
                        ProfileCommand();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void TeacherProblemsMenu()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.TeacherProblemMenu();
                cmd = view.ReadInt(maxValue:3);
                
                switch (cmd)
                {
                    case 1:
                        SelectProblems();
                        break;
                    case 2:
                        SelectProblems((CurrentUser as Teacher)?.MyProblems);
                        break;
                    case 3:
                        AddProblem();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void AddProblem()
        {
            Clear();
            string title = view.ReadString("Есептің тақырыбы: ");
            string text = view.ReadRichString();
            List<TestCase> cases = view.ReadTestCases();
            var problem = new Problem() {Title = title, Text = text, TestCases = cases};
            if (CurrentUser is Teacher teacher) teacher.MyProblems.Add(problem);
            // model.AppContext.Problems.Add(problem);
            model.AppContext.Update(CurrentUser);
            model.AppContext.Update(problem);
            model.AppContext.SaveChanges();
        }

        private void Search()
        {
            throw new NotImplementedException();
        }

        #region Director

        /*----Директор менюінің командасы----*/
        private void DirectorCommand()
        {
            while (true)
            {
                Clear();
                view.DirectorMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                var cmd = view.ReadInt(maxValue:2); 
                switch (cmd)
                {
                    case 1:
                        EmployeeInfoCommand();  // Жұмысшылар жайлы ақпаратты экранға шығару
                        break;
                    case 2:
                        IssueSalaryCommand();   // Жалақыны төлеу методы
                        break;
                    case 0:
                        return;
                }
            }
        }
        
        /*----Жұмысшылар жайлы ақпаратты экранға шығару----*/
        private void EmployeeInfoCommand()
        {
            // Барлық оқытушылар туралы ақпаратты экранға шығарамыз және бір оқытушыны таңдаймыз
            var teacher = view.Select(model.Users.Where(user => user.Role == Role.Teacher).ToList()) as Teacher;
            if (teacher == null) return;    // Егер оқытушы таңдалмаса, методтан шығамыз

            while (true)
            {
                view.Print(teacher);    // Оқытушы жайлы толық ақпаратты шығарамыз
                view.EmployeeInfoMenu();
                var cmd = view.ReadInt(maxValue: 2);
                switch (cmd)
                {
                    case 1:
                        ChangeSalary(teacher);  // Оқытушының жалақысын өзгерту методы
                        break;
                    case 2:
                        RemoveUser(teacher);    // Оқытушыны жүйеден жою методы
                        break;
                    case 0:
                        return;
                }
            }
        }
        /*----Оқытушының жалақысын өзгерту методы----*/
        private void ChangeSalary(Teacher teacher)
        {
            view.Print($"Бұрынғы жалақы: {teacher.Salary}\nЖаңа жалақыны енгізіңіз: ");
            double newSalary = view.ReadDouble();
            model.ChangeSalary(teacher, newSalary);
        }
        
        /*----Оқытушыны жүйеден жою методы----*/
        private void RemoveUser(Teacher teacher)
        {
            if (!view.YesOrNo(
                $"{teacher.Name} {teacher.LastName} пайдаланушы аккаунты жүйеден жойылады. Сенімдісіз бе?"))
                return;

            view.Println(model.RemoveUser(teacher)
                ? "Пайдаланушы сәтті жойылды! "
                : "Пайдаланушыны жою барысында қателік шықты! ");
            view.Wait();

        }
        
        /*----Жалақыны төлеу методы----*/
        private void IssueSalaryCommand()
        {
            Clear();
            var teachers = model.Users.OfType<Teacher>().ToList(); // Барлық оқытушыларды бөліп аламыз
            foreach (var teacher in teachers)       // Әрбір оқытушы үшін жалақыны есептейміз
            {
                double salary = teacher.Salary;
                AddBonus(teacher, ref salary);      // Артық есеп қосқаны үшін сыйақы жариялаймыз
                view.PrintSalary(teacher, salary);  // Жалақы өлшерін экранға шығарамыз
            }
            view.Wait();
        }

        private void AddBonus(Teacher teacher, ref double salary)
        {
            // Оқытушының орсы айда қосқан есептерін бөліп аламыз
            // Содан кейін оларды қосылған күні бойынша топтаймыз
            var days = teacher.MyProblems.Where(p=>p.Created.Month == DateTime.Today.Month).GroupBy(problem => problem.Created.Date);
            foreach (var day in days)       // Әрбір күн үшін сыйақыны есептейміз
            {
                // Егер осы күні қосылған есептер саны нормаға жетсе немесе асса
                if (day.ToList().Count >= ((Director) CurrentUser).ProblemCountPerDay)
                {
                    // Жалақыға сыйақы мөлшерін қосамыз
                    salary += ((Director) CurrentUser).DailyOverworkBonus;
                }
            }
        }

        #endregion
        
        #region Administrator

        public void AdministratorCommand()
        {
            int cmd;
            while (true)
            {
                view.AdministratorMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        AddTeacher();
                        break;
                    case 2:
                        RemoveUser();
                        break;
                    case 3:
                       ProfileCommand(); 
                        break;
                    
                    case 0:
                        return;
                }
            }
        }

        private void RemoveUser()
        {
            var user = view.Select(model.Users);
            if (user == null ||
                !view.YesOrNo($"{user.Name} {user.LastName} пайдаланушы аккаунты жүйеден жойылады. Сенімдісіз бе?"))
                return;
            if (model.RemoveUser(user))
            {
                view.Println("Пайдаланушы сәтті жойылды! ");
            } else view.Println("Пайдаланушыны жою барысында қателік шықты! ");
            view.Wait();
            
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
            
            Attempt attempt = model.AddAttemption(CurrentUser, problem);
            
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