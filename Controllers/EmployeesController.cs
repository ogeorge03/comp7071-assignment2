using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2;
using Assignment2.Models;
using Humanizer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            ", id).ToListAsync();

            ViewData["id"] = id;

            return View(shifts);
        }

        [HttpGet]
        public IActionResult CreateShift(int id)
        {
            var ss = _context.Shift_Schedules
                .Where(s => s.EmployeeId == id);

            ViewData["id"] = id;
            ViewData["sid"] = ss.FirstOrDefault()?.SupervisorId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShift(int id, DateTime Start_Datetime, int Hours_Scheduled, int Hours_Completed, string? Comments)
        {
            var ss = await _context.Shift_Schedules
                .Where(s => s.EmployeeId == id)
                .ToListAsync();

            if (ModelState.IsValid)
            {
                string sql = "INSERT INTO Shift_Schedule(EmployeeId, SupervisorId, Start_Datetime, Hours_Scheduled, Hours_Completed, Comments)" +
                    "VALUES({0}, {1}, {2}, {3}, {4}, {5})";
                int rowsAffected = _context.Database.ExecuteSqlRaw(sql, id, ss.FirstOrDefault()?.SupervisorId, Start_Datetime, Hours_Scheduled, Hours_Completed, Comments);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditShift(int? id, ShiftScheduleDetails viewModel)
        {
            var start_datetime = viewModel.Start_Datetime;

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
                        and Start_Datetime = {1}
            ", id, start_datetime).SingleOrDefaultAsync();

            var shift = await _context.Shift_Schedules.SingleOrDefaultAsync(s => s.EmployeeId == id && s.Start_Datetime == start_datetime);

            

            if (shifts == null)
            {
                return NotFound();
            }

            return View(shifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShift(int id, DateTime old_start_datetime, DateTime start_datetime, ShiftScheduleDetails viewModel)
        {
            var shift = await _context.Shift_Schedules.FirstOrDefaultAsync(s => s.EmployeeId == id);

            Shift_Schedule ss = new Shift_Schedule();
            ss.Start_Datetime = start_datetime;
            ss.Hours_Scheduled = viewModel.Hours_Scheduled;
            ss.Hours_Completed = viewModel.Hours_Scheduled;
            ss.Comments = viewModel?.Comments;

            if (ModelState.IsValid)
            {
                string sql = @"
                    UPDATE Shift_Schedule
                    SET Start_Datetime = {0}, 
                        Hours_Scheduled = {1}, 
                        Hours_Completed = {2}, 
                        Comments = {3}
                    WHERE EmployeeId = {4} 
                    AND Start_Datetime = {5}";

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql,
                    ss.Start_Datetime,
                    ss.Hours_Scheduled,
                    ss?.Hours_Completed,
                    ss?.Comments,
                    id,
                    old_start_datetime
                );

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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(viewModel.Id))
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
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteShift(int? id, ShiftScheduleDetails viewModel)
        {
            var start_datetime = viewModel.Start_Datetime;

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
                        and Start_Datetime = {1}
            ", id, start_datetime).SingleOrDefaultAsync();

            var shift = await _context.Shift_Schedules.SingleOrDefaultAsync(s => s.EmployeeId == id && s.Start_Datetime == start_datetime);

            ViewData["old_start_datetime"] = start_datetime;

            if (shifts == null)
            {
                return NotFound();
            }

            return View(shifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShift(int id, DateTime Start_Datetime, ShiftScheduleDetails viewModel)
        {
            var shift = await _context.Shift_Schedules.FirstOrDefaultAsync(s => s.EmployeeId == id);

            Shift_Schedule ss = new Shift_Schedule();
            ss.Start_Datetime = Start_Datetime;
            ss.Hours_Scheduled = viewModel.Hours_Scheduled;
            ss.Hours_Completed = viewModel.Hours_Scheduled;
            ss.Comments = viewModel?.Comments;

            if (ModelState.IsValid)
            {
                string sql = @"
                    DELETE FROM Shift_Schedule
                    WHERE EmployeeId = {0} 
                    AND Start_Datetime = {1}";

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, id, ss.Start_Datetime);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
            ", id).ToListAsync();
            
            if (shifts != null)
            {
                await _context.SaveChangesAsync();
            }
            
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

            var ev = _context.Employee_Vacations
                .Where(v => v.EmployeeId == id);

            var em = _context.Employees
                .Where(e => e.Id == id);

            var supervisorid = _context.Employees
                .Where(e => e.Id == id)
                .Select(e => e.Supervisor.Id);

            ViewData["id"] = id;
            ViewData["supervisorid"] = supervisorid;

            return View();
        }

        [HttpGet]
        public IActionResult CreateSickLeave(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSickLeave(int id, DateTime Sick_Day, string Doctors_Note)
        {

            if (ModelState.IsValid)
            {
                string sql = "INSERT INTO Employee_Sick_Leave(EmployeeId, Sick_Day, Doctors_Note)" +
                    "VALUES({0}, {1}, {2})";
                int rowsAffected = _context.Database.ExecuteSqlRaw(sql, id, Sick_Day, Doctors_Note);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditSickLeave(int? id, Employee_Sick_Leave viewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sick_day = viewModel.Sick_Day;

            var sickLeave = await _context.Employee_Sick_Leaves
                .Where(s => s.EmployeeId == id && s.Sick_Day == sick_day)
                .SingleOrDefaultAsync();

            if (sickLeave == null)
            {
                return NotFound();
            }
            return View(sickLeave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSickLeave(int? id, DateTime old_sick_day, Employee_Sick_Leave viewModel)
        {

            if (id == null)
            {
                return NotFound();
            }

            var sickLeave = await _context.Employee_Sick_Leaves
                .FirstOrDefaultAsync(s => s.EmployeeId == id && s.Sick_Day == old_sick_day);

            if (sickLeave != null)
            {
                _context.Employee_Sick_Leaves.Remove(sickLeave);
                _context.SaveChanges();

                var newSickLeave = new Employee_Sick_Leave
                {
                    EmployeeId = (int)id,
                    Sick_Day = viewModel.Sick_Day,
                    Doctors_Note = viewModel.Doctors_Note
                };

                _context.Employee_Sick_Leaves.Add(newSickLeave);
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
                    //_context.Update(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(sickLeave.EmployeeId))
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
            return View(sickLeave);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSickLeave(int? id, Employee_Sick_Leave viewModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sick_day = viewModel.Sick_Day;

            var sickLeave = await _context.Employee_Sick_Leaves
                .Where(s => s.EmployeeId == id && s.Sick_Day == sick_day)
                .SingleOrDefaultAsync();

            if (sickLeave == null)
            {
                return NotFound();
            }
            return View(sickLeave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSickLeave(int? id, DateTime Sick_Day, Employee_Sick_Leave viewModel)
        {

            if (id == null)
            {
                return NotFound();
            }

            var sickLeave = await _context.Employee_Sick_Leaves
                .FirstOrDefaultAsync(s => s.EmployeeId == id && s.Sick_Day == Sick_Day);

            if (sickLeave != null)
            {
                _context.Employee_Sick_Leaves.Remove(sickLeave);
                await _context.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(sickLeave.EmployeeId))
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
            return View(sickLeave);
        }

        [HttpGet]
        public IActionResult CreateVacation(int? id)
        {
            var supervisorid = _context.Employees
                .Where(e => e.Id == id)
                .Select(e => e.SupervisorId)
                .FirstOrDefault();

            var vacation = _context.Employee_Vacations
                .Where(v => v.EmployeeId == id);

            ViewData["supervisorid"] = supervisorid;
            ViewData["id"] = id;

            return View();
        }

        public async Task<IActionResult> CreateVacation(int? id, int supervisorid, Employee_Vacation createModel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisor = await _context.Employees.FirstOrDefaultAsync(e => e.Id == supervisorid);

            var vacation = new Employee_Vacation
            {
                EmployeeId = (int)id,
                Supervisor = supervisor,
                SupervisorId = supervisorid,
                Vacation_Start_Date = createModel.Vacation_Start_Date,
                Vacation_End_Date = createModel.Vacation_End_Date,
                Is_SuperVisor_Approved = createModel.Is_SuperVisor_Approved,
                Is_Paid_Vacation = createModel.Is_Paid_Vacation,
                Approval_Date = createModel.Approval_Date
            };

            _context.Employee_Vacations.Add(vacation);
            _context.SaveChanges();

            if (vacation == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditVacation(int? id, int? employeeid)
        {

            if (id == null)
            {
                return NotFound();
            }

            var vacation = await _context.Employee_Vacations
                .Where(ev => ev.Id == id && ev.EmployeeId == employeeid)
                .SingleOrDefaultAsync();

            return View(vacation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVacation(int? id, int? employeeid, Employee_Vacation viewModel)
        {
            if (employeeid == null)
            {
                return NotFound();
            }

            var vacation = await _context.Employee_Vacations
                .SingleOrDefaultAsync(ev => ev.EmployeeId == employeeid && ev.Id == id);

            if (vacation != null)
            {
                vacation.Id = (int)id;
                vacation.EmployeeId = viewModel.EmployeeId;
                vacation.Vacation_Start_Date = viewModel.Vacation_Start_Date;
                vacation.Vacation_End_Date = viewModel.Vacation_End_Date;
                vacation.Is_SuperVisor_Approved = viewModel.Is_SuperVisor_Approved;
                vacation.Is_Paid_Vacation = viewModel.Is_Paid_Vacation;
                vacation.Approval_Date = viewModel.Approval_Date;

                await _context.SaveChangesAsync();
                return RedirectToAction("WorkHistory", new { id = employeeid });
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
                    if (!EmployeeExists(vacation.EmployeeId))
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
            return View(vacation);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVacation(int? id, int? employeeid)
        {

            if (id == null)
            {
                return NotFound();
            }

            var vacation = await _context.Employee_Vacations
                .Where(ev => ev.Id == id && ev.EmployeeId == employeeid)
                .SingleOrDefaultAsync();

            return View(vacation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVacation(int? id, int? employeeid, Employee_Vacation viewModel)
        {
            if (employeeid == null)
            {
                return NotFound();
            }

            var vacation = await _context.Employee_Vacations
                .SingleOrDefaultAsync(ev => ev.EmployeeId == employeeid && ev.Id == id);

            if (vacation != null)
            {
                _context.Employee_Vacations.Remove(vacation);
                await _context.SaveChangesAsync();
                return RedirectToAction("WorkHistory", new { id = employeeid });
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
                    if (!EmployeeExists(vacation.EmployeeId))
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
            return View(vacation);
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
        public async Task<IActionResult> Create(Employee createModel)
        {
            var newEmployee = new Employee
            {
                Job_Title = createModel.Job_Title,
                Pay_Rate_Amount = createModel.Pay_Rate_Amount,
                Employment_Start_Date = createModel.Employment_Start_Date,
                First_Name = createModel.First_Name,
                Middle_Name = createModel.Middle_Name,
                Last_Name = createModel.Last_Name,
                Date_Of_Birth = createModel.Date_Of_Birth
            };

            _context.Add(newEmployee);
            _context.SaveChanges();
         
            return View();
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
