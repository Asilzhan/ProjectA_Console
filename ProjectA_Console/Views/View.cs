using System;
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
            Console.ForegroundColor = color;
            int res;
            while (!int.TryParse(Console.ReadLine(), out res))
            {
                ShowError();
                Print($"{key} ");
            }

            return res;
        }
        
        public void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = c;
        }

        public string ReadString(string key = "string", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = Console.ForegroundColor;
            Console.ForegroundColor = color;
            string res = Console.ReadLine();
            Console.ForegroundColor = f;
            return res;
        }
        
        public void ShowError(string afterText = "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Қате, қайтадан енгізіңіз: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(afterText);
        }
    }
}