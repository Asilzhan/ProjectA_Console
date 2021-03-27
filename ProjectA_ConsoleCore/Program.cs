using System;
using System.Globalization;
using System.Text;

namespace ProjectA_ConsoleCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.Unicode;
            /*-----Культураны қазақшаға ауыстыру-----*/
            CultureInfo.CurrentCulture = new CultureInfo(0x043F);
            Controller.Controller controller = new Controller.Controller();
            controller.Main();
        }
    }
}