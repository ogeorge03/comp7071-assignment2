using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly Context _context;

        public EmployeesController(Context context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {

            var employees = await _context.EmployeesDetails.FromSqlRaw(@"
                            select 
                            p.id
                            ,p.First_Name as [FirstName]
                            ,p.Middle_Name as [MiddleName]
                            ,p.Last_Name as [LastName]
                            ,p.Date_Of_Birth as [DateOfBirth]
                            ,stadd.Street_Name as [StreetName]
                            ,stadd.City
                            ,stadd.Postal_Code as [PostalCode]
                            ,stadd.Country
                            ,(select Legal_Name from Org where Id = e.EmployerId) as [Employer]
                            ,(select First_Name + ' ' + Last_Name from Person where Id = e.Emergency_ContactId) as [EmergencyContact]
                            ,e.Job_Title as [JobTitle]
                            ,e.Employment_Start_Date as [EmploymentStartDate]
                            ,e.Employment_Termination_Date as [EmploymentTerminationDate]
                            ,e.Pay_Rate_Amount as [PayRateAmount]
                            from Employee e
                            inner join Person p on p.Id = e.Id
                            inner join StAddress stadd on stadd.Id = p.Primary_ResidenceId
            ").AsNoTracking().ToListAsync();

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.EmployeesDetails.FromSqlRaw(@"
                            select 
                            p.id
                            ,p.First_Name as [FirstName]
                            ,p.Middle_Name as [MiddleName]
                            ,p.Last_Name as [LastName]
                            ,p.Date_Of_Birth as [DateOfBirth]
                            ,stadd.Street_Name as [StreetName]
                            ,stadd.City
                            ,stadd.Postal_Code as [PostalCode]
                            ,stadd.Country
                            ,(select Legal_Name from Org where Id = e.EmployerId) as [Employer]
                            ,(select First_Name + ' ' + Last_Name from Person where Id = e.Emergency_ContactId) as [EmergencyContact]
                            ,e.Job_Title as [JobTitle]
                            ,e.Employment_Start_Date as [EmploymentStartDate]
                            ,e.Employment_Termination_Date as [EmploymentTerminationDate]
                            ,e.Pay_Rate_Amount as [PayRateAmount]
                            from Employee e
                            inner join Person p on p.Id = e.Id
                            inner join StAddress stadd on stadd.Id = p.Primary_ResidenceId
                            where e.Id = {0}
            ", id).AsNoTracking().FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> Shifts(int? id) {
            if (id == null)
            {
                return NotFound();
            }

            var shifts = await _context.ShiftScheduleDetails.FromSqlRaw(@"
                        select 
                        0 as id
                        ,Start_Datetime
                        ,Hours_Scheduled
                        ,Hours_Completed
                        ,Comments
                        , (select First_Name + ' ' + Last_Name from Person where Id = SupervisorId) as [Supervisor]
                        from Shift_Schedule
                        where EmployeeId = {0}
            ", id).AsNoTracking().ToListAsync();


            return View(shifts);
        }

        public async Task<IActionResult> Payroll(int? id) {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FromSqlRaw(@"
                        select 
                        *      
                        from Payroll
                        where EmployeeId = {0}
            ", id).AsNoTracking().ToListAsync();


            return View(payroll);
        }

        public async Task<IActionResult> WorkHistory(int? id) {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["statHolidays"] = await _context.Stat_Holidays.FromSqlRaw(@"
                        select * from stat_holidays
            ").AsNoTracking().ToListAsync();

            ViewData["vacations"] = await _context.Employee_Vacations.FromSqlRaw(@"
                        select * from Employee_Vacation where EmployeeId = {0}
            ", id).AsNoTracking().ToListAsync();

            ViewData["sickLeave"] = await _context.Employee_Sick_Leaves.FromSqlRaw(@"
                        select * from Employee_Sick_Leave where EmployeeId = {0}
            ", id).AsNoTracking().ToListAsync();

            return View();
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Job_Title,Pay_Rate_Amount,Employment_Start_Date,Employment_Termination_Date,First_Name,Middle_Name,Last_Name,Date_Of_Birth")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee viewModel)
        {

            var employee = await _context.Employees.FindAsync(viewModel.Id);

            if (employee != null)
            {
                employee.First_Name = viewModel.First_Name;
                employee.Middle_Name = viewModel.Middle_Name;
                employee.Last_Name = viewModel.Last_Name;
                employee.Date_Of_Birth = viewModel.Date_Of_Birth;
                employee.Employer = viewModel.Employer;
                employee.Emergency_Contact = viewModel.Emergency_Contact;
                employee.Job_Title = viewModel.Job_Title;
                employee.Employment_Start_Date = viewModel.Employment_Start_Date;
                employee.Employment_Termination_Date = viewModel.Employment_Termination_Date;
                employee.Pay_Rate_Amount = viewModel.Pay_Rate_Amount;

                await _context.SaveChangesAsync();
            }

            if (viewModel == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
