using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ProjectA_Console.Models
{
    public class Attempt
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime ShippingTime { get; set; }
        public Problem ProblemId { get; set; }
        public List<TestCase> TestCases { get; set; }
        public Verdict Verdict { get; set; }
    }
}