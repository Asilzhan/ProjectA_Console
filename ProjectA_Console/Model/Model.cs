using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectA_Console.Model
{
    public class Model
    {
        private List<Administrator> Administrators = new List<Administrator>()
        {
            new Administrator("ddd", "almat", "s", DateTime.Now, "dd", "ddd", "df"),
            new Administrator("ddd", "almat", "s", DateTime.Now, "dd", "ddd", "df"),
        };

        private List<Student> Students = new List<Student>()
        {
            new Student("fgh", "dfh", "dfh", DateTime.Today, "dsf", "Sdg", 3, "Sg", "Dfh"),
            new Student("fgh", "dfh", "dfh", DateTime.Today, "dsf", "Sdg", 3, "Sg", "Dfh"),
        };
    }


}