using System;
using System.Globalization;
using  static System.Console;

namespace ProjectA_Console.Views
{
    public class View
    {
        public void MainMenu()
        {
            WriteLine("[1] - Кіру");
            WriteLine("[2] - Тіркелу");
        }
        
        public void StudentMenu()
        {
            WriteLine("[1] - Есептер");
        }

        public void ProblemMenu()
        {
            WriteLine("");
        }
        
        public int ReadInt(string key = "string", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = Console.ForegroundColor;
            ForegroundColor = color;
            int res;
            while (!int.TryParse(Console.ReadLine(), out res))
            {
                ShowError();
                Print($"{key} ");
            }

            return res;
        }

        public DateTime ReadDate(string key = "string", ConsoleColor color = ConsoleColor.White)
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
            ConsoleColor f = Console.ForegroundColor;
            ForegroundColor = color;
            string res = Console.ReadLine();
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
    }
}