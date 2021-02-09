using System.Collections.Generic;

namespace ProjectA_Console.Model
{
    public class Problem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TestCase> TestCases { get; set; }
    }
}