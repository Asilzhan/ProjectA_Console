using System;

namespace ProjectA_Console.View.MenuProvider
{
    class MenuItem
    {
        public bool IsMenu { get; set; }
        public string Title { get; set; }
        public Action Action { get; set; }
        public Menu InnerMenu { get; set; }
    }
}