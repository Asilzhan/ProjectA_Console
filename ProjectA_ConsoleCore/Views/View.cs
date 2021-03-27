using System;
using System.Collections.Generic;
using ProjectA_ConsoleCore.Helper;
using ProjectA_ConsoleCore.Models;
using  static System.Console;

namespace ProjectA_ConsoleCore.Views
{
    public class View
    {
        #region Menu

        // Директордың бас менюі
        public void DirectorMenu(string name = "")
        {
            ShowHappy(name);
            WriteLine("[1] - Жұмысшылар туралы ақпарат\n" +
                      "[2] - Айлық есептеу\n" +
                      "[0] - Артқа");
        }
        // Жұмысшы туралы меню
        public void EmployeeInfoMenu()
        {
            WriteLine("[1] - Жалақысын өзгерту\n" +
                      "[2] - Жұмыстан шығару\n" +
                      "[0] - Артқа");
        }
        public void MainMenu()
        {
            WriteLine("[1] - Кіру\n" +
                      "[2] - Тіркелу\n" +
                      "[0] - Шығу");
        }

        public void ProfileMenu(string name)
        {
            Clear();
            ShowHappy(name);
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
        
        public void AdministratorMenu(string name)
        {
            Clear();
            ShowHappy(name, Role.Administrator);
            WriteLine("[1] - Мұғалім қосу\n" +
                      "[2] - Қолданушыны жою\n" +
                      "[3] - Профиль\n" +
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
        
        public void ShowHappy(string name, Role role = Role.Student)
        {
            if (role == Role.Administrator)
                ForegroundColor = ConsoleColor.Magenta;
            else 
                ForegroundColor = ConsoleColor.Green;
            WriteLine($"Қош келдіңіз, {name}!");
            ForegroundColor = ConsoleColor.White;
        }
        
        public void GoodBye()
        {
            Clear();
            Print("Сау болыңыз!", ConsoleColor.Green);
        }
        
        #endregion

        #region Read
        
        public double ReadDouble(string key = "double", double minValue = 0, double maxValue = double.MaxValue, ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}>> ");

            var f = ForegroundColor;
            ForegroundColor = color;
            double res=-1;
            while (!double.TryParse(ReadLine(), out res) || res > maxValue || res < 0)
            {
                ShowError();
                Print($"{key}>> ");
            }

            return res;
        }

        public int ReadInt(string key = "int", int maxValue = Int32.MaxValue, ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}>> ");

            var f = ForegroundColor;
            ForegroundColor = color;
            int res=-1;
            while (!int.TryParse(ReadLine(), out res) || res > maxValue || res < 0)
            {
                ShowError();
                Print($"{key}>> ");
            }

            return res;
        }
        public string ReadPass(string message = "", ConsoleColor color = ConsoleColor.White)
        {
            return User.GetHashString(ReadString(message == "" ? "Пароль: " : message, color).Trim());
        }
        public DateTime ReadDate(string key = "", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = ForegroundColor;
            ForegroundColor = color;
            DateTime res;
            while(!DateTime.TryParse(ReadLine(), out res))
            {
                ShowError();
                Print($"{key} ");
            }
            return res;
        }

        public string ReadString(string key = "string", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}");
            var f = ForegroundColor;
            ForegroundColor = color;
            var res = ReadLine();
            ForegroundColor = f;
            return res;
        }

        public string ReadRichString(string key = "Есептің берілгенін ашылған терезеге жазып, сақтаңыз (Ctrl+S). Есептің берілгенін жазып болған соң ашылған терезені жабыңыз (Enter - OK)")
        {
            Print($"{key} ");
            ReadKey();
            WriteLine();
            FileHelper fh = new FileHelper();
            string text = fh.GetTextFromEditor();
            return text;
        }
        
        public List<TestCase> ReadTestCases()
        {
            List<TestCase> cases = new List<TestCase>();
            do
            {
                Clear();
                var input = ReadString("Тесттің кіріс мәндерін бір қатарға бос орын арқылы бөліп жазыңыз:\n");
                var output = ReadString("Тесттің шығыс мәндерін бір қатарға бос орын арқылы бөліп жазыңыз:\n");
                cases.Add(new TestCase(input, output));
                
            } while (YesOrNo("Тағы бір тест қосқыңыз келеді ме? "));

            return cases;
        }
        
        public bool YesOrNo(string message="")
        {
            string choice;
            do
            {
                choice = ReadString($"{message} (y/n) ").ToLower();
            } while (choice != "y" && choice != "n");

            return choice == "y";
        }

        public void Wait() => ReadKey();
        

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

        public void Println(string text, ConsoleColor color = ConsoleColor.White) => Print(text + '\n', color);
        
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
            WriteLine($"+------------------------------+\n");
        }
        /*----Оқытушы туралы ақпратты эканға шығару методы----*/
        public void Print(Teacher teacher)
        {
            Print(teacher as User);     // Жалпы ақпарат
            // Және қосымша ақпарат
            WriteLine($"|Жалақы: {teacher.Salary, 22:C}|");
            WriteLine($"|Есеп қосты: {teacher.MyProblems.Count, 18}|");
            WriteLine($"+------------------------------+\n");
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

        public T Select<T>(List<T> list) where T : class
        {
            Clear();
            // string t = new string('-', BufferWidth);
            for (int i = 0; i < list.Count; i++)
            {
                Println($"{i+1}) {list[i]}");
                // Print(t, ConsoleColor.DarkBlue);
            }
            Println("0) Артқа");
            // Print(t, ConsoleColor.DarkBlue);
            
            int index = ReadInt("Таңдаңыз: ", list.Count);
            if (index == 0) return default;
            return list[index-1];
        }
        
        #endregion

        public void PrintSalary(Teacher teacher, double salary)
        {
            Println("Берілетін жалақы мөлшері: ");
            Println($"{teacher.LastName} {teacher.Name}, {salary:C0}");
        }
    }
}