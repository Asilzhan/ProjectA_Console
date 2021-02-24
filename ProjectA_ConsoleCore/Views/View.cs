using System;
using System.Collections.Generic;
using System.Globalization;
using ProjectA_ConsoleCore.Models;
using  static System.Console;

namespace ProjectA_ConsoleCore.Views
{
    public class View
    {
        public void MainMenu()
        {
            WriteLine("[1] - Кіру");
            WriteLine("[2] - Тіркелу");
            WriteLine("[0] - Шығу");
        }

        public void StudentMenu()
        {
            WriteLine("[1] - Есептер");
            WriteLine("[0] - Артқа");
        }

        public void ProblemMenu()
        {
            WriteLine("[1] - Жіберу");
            WriteLine("[2] - Менің жауаптарым");
            WriteLine("[0] - Артқа");
        }

        public void Print(List<Problem> problems)
        {
            Print("Есепті таңдаңыз:\n", ConsoleColor.Green);
            for (int i = 0; i < problems.Count; i++)
            {
                WriteLine($"{i}) {problems[i]}");
            }
        }
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
        public int ReadPass(ConsoleColor color = ConsoleColor.White)
        {
            Print($"Пароль: ");
            var f = ForegroundColor;
            ForegroundColor = color;
            string res = ReadLine();

            return res.GetHashCode();
        }
        public DateTime ReadDate(string key = "", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = Console.ForegroundColor;
            ForegroundColor = color;
            DateTime res;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd:MM:yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
            {
                ShowError();
                Print($"{key} ");
            }
            return res;
        }
        
        public void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor c = Console.ForegroundColor;

            ForegroundColor = color;
            Write(text);
            ForegroundColor = c;
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

        public void Print(List<Attempt> attempts)
        {
            if(attempts==null || attempts.Count==0) Print("Сіз ештеңе жібермегенсіз");
            else
            {
                Print($"{"ID",5} {"Аты", 20} {"Уақты", 10} {"Вердикт", 15}\n");
                foreach (var attempt in attempts)
                {
                    Print($"{attempt.Id,5} {attempt.Problem.Title, 20} {attempt.ShippingTime:d, 10} {attempt.Verdict, 10}\n");
                }

                ReadKey();
            }
        }
        public void Print(Problem problem)
        {
            WriteLine(new string('-',120));
            WriteLine($"{"",55}{problem.Title}");
            WriteLine(new string('-',120));
            WriteLine($"{problem.Text}");
            WriteLine(new string('-',120));
            WriteLine(String.Format("|{0, 58}|{1,58}|", "Input", "Output"));
            foreach (var problemTestCase in problem.TestCases)
            {
                WriteLine($"|{problemTestCase.Input.Trim(),58}|{problemTestCase.Output.Trim(),58}|");
            }
            WriteLine(new string('-',120));
        }
    }
}