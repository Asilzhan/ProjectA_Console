using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        Compiler compiler = new Compiler();
        public User CurrentUser;
       
        #region Main

        public void Main()
        {
            compiler.StatusChanged += view.Rewrite;
            
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
                view.TeacherMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt(maxValue: 4);
                
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
                    // Мониторинг жүргізуге арналган метод
                    case 4: 
                        MonitoringStudentProgress();
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

        private void MonitoringStudentProgress()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.MonitoringStudents(model.Users.OfType<Student>().ToList(), model.Problems); // Мониторинг кестесі
                view.SendMessageToTheStudentMenu(); 
                cmd = view.ReadInt(maxValue:2);
                
                switch (cmd)
                {
                    case 0:
                        return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
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

        public void Submit(Problem problem)
        {
            string sourcePath = view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе тышқанмен сүйреп әкеліңіз\n", ConsoleColor.Green);
            sourcePath = sourcePath.Trim('\'', '\"');
            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }
            Attempt attempt = model.AddAttemption(CurrentUser, problem);
            
            Thread thread = new Thread(o =>
            {
                compiler.Sumbit(attempt, problem, sourcePath);
                // әрбір жіберілген попыткаларды базада сақтау
                model.AppContext.SaveChanges();
            });
            thread.Start();
            UserAttemptsPage();
            // compiler.Sumbit(model, CurrentUser, problem, sourcePath);
            // // әрбір жіберілген попыткаларды базада сақтау
            // model.AppContext.SaveChanges();
            
            
            // TODO: model.Attempts.Add(attempt);
        }
        
        #endregion

        protected void UserAttemptsPage()
        {
            Clear();
            view.Print(CurrentUser.Attempts);
            view.Print("Артқа оралу үшін Enter басыңыз...");
            ReadKey();
        }

    }
}