using System.Collections.Generic;

namespace ProjectA_Console.Models
{
    public class Classroom
    {
        public Teacher Teacher { get; set; }
        public List<Student> Students { get; set; }
        
    }
}