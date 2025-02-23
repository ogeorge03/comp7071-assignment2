using System;

namespace Assignment2.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string First_Name { get; set; }
        public string Middle_Name { get; set; }

        public string Last_Name { get; set; }
        public DateTime Date_Of_Birth { get; set; }

        public StAddress? Primary_Residence { get; set; }

    }
}
