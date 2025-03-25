﻿
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class Employee : Person
    {
        public int Id { get; set; }
        public Employee? Supervisor { get; set; }
        public int SupervisorId { get; set; }
        public Org Employer { get; set; }
        public Person? Emergency_Contact { get; set; }
        public string Job_Title { get; set; }
        public enum EMPLOYMENT_TYPE { FULL_TIME=0, PART_TIME, ON_CALL }
        public enum SALARY_HOURLY { SALARY=0, HOURLY}
        public decimal Pay_Rate_Amount {  get; set; }

        public enum PAY_RATE_FREQUENCY { MONTHLY=0, WEEKLY=1, BI_WEEKLY=2 }

        public enum WORK_STATUS { ACTIVE=0, TERMINATED=1, ON_LEAVE=2}

        public DateTime Employment_Start_Date { get; set; }

        public DateTime? Employment_Termination_Date { get; set; }
        public ICollection<Employee_Certification> Employee_Certifications { get; set; }
        public ICollection<Employee_Service_Assignment> Employee_Service_Assignments { get; set; }

        [NotMapped]
        public ICollection<Employee_Sick_Leave> Employee_Sick_Leaves { get; set; }
        public ICollection<Employee_Vacation> Employee_Vacations { get; set; }
    }
}
