﻿using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Teacher : User
    {
        public List<Problem> MyProblems { get; set; }
        public Teacher(string name, string lastName, DateTime birthday, string login, string passwordHash) : base(
            name, lastName,
            birthday, login, passwordHash)
        {
            Role = Role.Teacher;
            MyProblems = new List<Problem>();
        }
    }
}