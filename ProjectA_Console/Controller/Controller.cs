using System;
using System.Threading;
using ProjectA_Console.Models;
using ProjectA_Console.Views;
using static System.Console;

namespace ProjectA_Console.Controller
{
    public class Controller
    {
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
                    default: return;
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
        }
    }
}