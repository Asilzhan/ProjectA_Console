using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectA_Console.Models;
using static System.Console;

namespace ProjectA_Console.Views
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
            WriteLine("");
        }

        public void Print(List<Problem> problems)
        {
            Print("Есепті таңдаңыз: ", ConsoleColor.Green);
            for (int i = 0; i < problems.Count; i++)
            {
                WriteLine($"{i}) {problems[i]}");
            }
        }
        public int ReadInt(string key = "string", int maxValue = Int32.MaxValue, ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            var f = ForegroundColor;
            ForegroundColor = color;
            int res;
            while (!int.TryParse(ReadLine(), out res) && res >= maxValue)
            {
                ShowError();
                Print($"{key} ");
            }

            return res;
        }

        public void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            var c = ForegroundColor;
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
    }
}