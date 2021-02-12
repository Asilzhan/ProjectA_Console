using System;
using ProjectA_Console.Models;
using ProjectA_Console.Views;
using static System.Console;

namespace ProjectA_Console.Controller
{
    public class Controller
    {
        View view = new View();
        Model model = new Model();

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
                    case 2:
                    case 3:
                      default: return;
                }
            }
        }
    }
}