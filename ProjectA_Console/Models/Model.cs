﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectA_Console.Models
{
    public class Model
    {
        public  List<Student> Students { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Problem> Problems { get; set; }
        public List<Attempt> Attempts { get; set; }
        
        public Model()
        {
            Students = new List<Student>();
            Teachers = new List<Teacher>();
            Problems = new List<Problem>();
            Attempts = new List<Attempt>();
        }
        
        public List<Problem> GetProblemsByTeacherId(int teacherId)
        {
            return Problems.FindAll(problem => problem.Id == teacherId);
        }

        public bool TryAddStudent(string name, string lastName, DateTime born, int course, string login, int passHash)
        {
            if (Students.Exists(s => (s.Name == name && s.LastName == lastName) || s.Login == login))
            {
                return false;   // Бұндай атты немесе логинді студент бар болса
            }
            int id = Students.Max(s => s.Id);
            Student student = new Student(id, name, lastName, born, course, login, passHash);
            Students.Add(student);
            return true;
        }
        
        public Problem AddProblem(string title, string text)
        {
            int id = Problems.Max(s => s.Id);
            Problem problem = new Problem() {Id = id, Title = title, Text = text};
            return problem;
        }
    }
}