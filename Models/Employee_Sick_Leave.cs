﻿namespace MVCSampleApp.Models
{
    public class Employee_Sick_Leave
    {
        public Employee Employee { get; set; }
        public DateTime Sick_Day { get; set; }
        public string? Doctors_Note { get; set; }

    }
}
