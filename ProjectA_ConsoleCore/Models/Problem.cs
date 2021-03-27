﻿using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TestCase> TestCases { get; set; }
        public DateTime Created { get; set; }   // Есептің жасалған уақытын сипаттайтын қасиет
        public void AddTestCase(string input, string output)
        {
            TestCases.Add(new TestCase(input, output));
        }

        public override string ToString()
        {
            return $"{Title} [id={Id}]";
        }
    }
}