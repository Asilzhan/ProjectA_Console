using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProjectA_ConsoleCore.Models;
using  static System.Console;

namespace ProjectA_ConsoleCore.Views
{
    public class View
    {
        #region Menu

        public void MainMenu()
        {
            WriteLine("[1] - Кіру\n" +
                      "[2] - Тіркелу\n" +
                      "[0] - Шығу");
        }

        public void ProfileMenu()
        {
            WriteLine("[1] - Іздеу\n" +
                      "[2] - Есептер\n" +
                      "[3] - Профиль\n" +
                      "[0] - Артқа");
        }
        public void StudentProblemMenu()
        {
            WriteLine("[1] - Жіберу\n" +
                      "[2] - Менің жауаптарым\n" +
                      "[0] - Артқа");
        }

        public void TeacherProblemMenu()
        {
            WriteLine("[1] - Барлық есептер\n" +
                      "[2] - Менің есептерім\n" +
                      "[3] - Есеп қосу\n" +
                      "[0] - Артқа");
        }
        
        public void AdministratorMenu()
        {
            WriteLine("[1] - Мұғалім қосу\n" +
                      "[2] - Профиль\n" +
                      "[0] - Артқа");
        }
        
        public void EditMenu()
        {
            WriteLine("[1] - Парольді өзгерту\n" +
                      "[0] - Артқа");
        }
        #endregion

        #region Emodji

        public void ShowError(string afterText = "")
        {
            ForegroundColor = ConsoleColor.Red;
            Write("Қате, қайтадан енгізіңіз: ");
            ForegroundColor = ConsoleColor.White;
            WriteLine(afterText);
        }
        
        public void ShowHappy(string name)
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Құттықтаймыз, {name} жүйеге сәтті кірдіңіз!!!");
            ForegroundColor = ConsoleColor.White;
        }
        
        public void GoodBye()
        {
            Clear();
            Print("Сау болыңыз!", ConsoleColor.Green);
        }
        #endregion

        #region Read
        
        public int ReadInt(string key = "int", int maxValue = Int32.MaxValue, ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}>> ");

            var f = ForegroundColor;
            ForegroundColor = color;
            int res=-1;
            while (!int.TryParse(ReadLine(), out res) || res >= maxValue)
            {
                ShowError();
                Print($"{key}>> ");
            }

            return res;
        }
        public string ReadPass(ConsoleColor color = ConsoleColor.White)
        {
            Print($"Пароль: ");
            var f = ForegroundColor;
            ForegroundColor = color;
            string res = ReadLine().Trim();

            return User.GetHashString(res);
        }
        public DateTime ReadDate(string key = "", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = ForegroundColor;
            ForegroundColor = color;
            DateTime res;
            while(!DateTime.TryParse(ReadLine(), out res))
            // while (!DateTime.TryParseExact(ReadLine(), "dd:MM:yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
            {
                ShowError();
                Print($"{key} ");
            }
            return res;
        }

        public string ReadString(string key = "string", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            var f = ForegroundColor;
            ForegroundColor = color;
            var res = ReadLine();
            ForegroundColor = f;
            return res;
        }
        #endregion
        
        #region Print

        public void Print(List<Attempt> attempts)
        {
            if (attempts == null || attempts.Count == 0)
            {
                Print("Сіз ештеңе жібермегенсіз");
                ReadKey();
            }
            else
            {
                WriteLine(new string('-', 70));
                WriteLine($"|{"ID",5}|{"Аты", 20}|{"Уақыты", 20}|{"Вердикт", 20}|");
                WriteLine(new string('-', 70));
                foreach (var attempt in attempts)
                {
                    WriteLine($"|{attempt.Id,5}|{attempt.Problem.Title, 20}|{attempt.ShippingTime, 20}|{attempt.Verdict, 20}|");
                }
                WriteLine(new string('-', 70));
                ReadKey();
            }
        }
        
        public void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor c = ForegroundColor;

            ForegroundColor = color;
            Write(text);
            ForegroundColor = c;
        }

        public void Print(List<Problem> problems)
        {
            Print("Есепті таңдаңыз:\n", ConsoleColor.Green);
            for (var i = 0; i < problems.Count; i++)
            {
                WriteLine($"{i}) {problems[i]}");
            }
        }
        
        public void Print(Problem problem)
        {
            WriteLine(new string('-',120));
            WriteLine($"{"",55}{problem.Title}");
            WriteLine(new string('-',120));
            WriteLine($"{problem.Text}");
            WriteLine(new string('-',120));
            WriteLine($"|{"Input",58}|{"Output",58}|");
            foreach (var problemTestCase in problem.TestCases)
            {
                WriteLine($"|{problemTestCase.Input.Replace('\n', ' '),58}|{problemTestCase.Output.Replace('\n', ' '),58}|");
            }
            WriteLine(new string('-',120));
        }

        #endregion

        #region Select

        public T Select<T>(T[] list)
        {
            Clear();
            for (int i = 0; i < list.Length; i++)
            {
                Print($"{list[i]}\n");
                Print(new string('-', BufferWidth), ConsoleColor.DarkBlue);
            }

            return list[ReadInt("Таңдаңыз: ", list.Length)];

        }
        
        public T Select<T>(List<T> list)
        {
            Clear();
            for (int i = 0; i < list.Count; i++)
            {
                Print($"{i+1}) {list[i]}\n");
                Print(new string('-', BufferWidth), ConsoleColor.DarkBlue);
            }

            return list[ReadInt("Таңдаңыз: ", list.Count)];
        }

        public void Print(User user)
        {
            WriteLine($"+------------------------------+");
            WriteLine($"|             Ақпарат          |");
            WriteLine($"+------------------------------+");
            WriteLine($"|Аты: {user.Name, 25}|");
            WriteLine($"|Фамилия: {user.LastName, 21}|");
            WriteLine($"|Туған күні: {user.Birthday, 18:d}|");
            WriteLine($"|Логин: {user.Login, 23}|");
            WriteLine($"+------------------------------+");
            WriteLine($"|{user.Role, 20}          |");
            WriteLine($"+------------------------------+");
        }
        #endregion

       
    }
}