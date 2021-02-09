using System;
using ProjectA_Console.View;
using ProjectA_Console.View.MenuProvider;

namespace ProjectA_Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Controller.Controller controller = new Controller.Controller();
            MenuBuilder builder = new MenuBuilder();
            var menu = builder.Build("menu.xml", controller);


            foreach (var menuItem in menu.MenuItems)
            {
                Obhod(menuItem);
            }
            Console.ReadKey();
        }

        public static void Obhod(MenuItem item)
        {
            if (item.IsMenu)
            {
                foreach (var menuItem in item.InnerMenu.MenuItems)
                {
                    Obhod(menuItem);
                }
            } else
            {
                Console.WriteLine($"----- {item.Title}");
                item.Action?.Invoke();
            }
        }
    }
}