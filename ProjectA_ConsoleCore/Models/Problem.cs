using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TestCase> TestCases { get; set; }

        public int Point { get; set; }

        public void AddTestCase(string input, string output)
        {
            TestCases.Add(new TestCase(input, output));
        }

        public override string ToString()
        {
            return ($" |{Id} |{Title,20}| {Point}|").PadLeft(5);
        }
    }
}