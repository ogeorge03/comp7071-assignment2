using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Controllers
{
    public class PayrollController : Controller
    {
        private readonly Context _context;

        public PayrollController(Context context)
        {
            _context = context;
        }

        // GET: Payroll
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var payroll = await _context.Payrolls.FromSqlRaw(@"
                        select 
                        *      
                        from Payroll
                        where EmployeeId = {0}
            ", id).AsNoTracking().ToListAsync();

            ViewData["employeeid"] = id;

            return View(payroll);
        }

        // GET: Payroll/Create
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = _context.Payrolls
                .Where(p => p.EmployeeId == id);

            ViewData["employeid"] = id;

            return View();              
        }

        // POST: Payroll/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, Payroll new_payroll)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pr = await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeId == id);

            var payroll = new Payroll()
            {
                EmployeeId = (int)id,
                Employee = pr?.Employee,
                Processed_Datetime = new_payroll?.Processed_Datetime,
                Regular_Pay = new_payroll.Regular_Pay,
                Overtime_Pay = new_payroll.Overtime_Pay,
                Vacation_Pay = new_payroll.Vacation_Pay,
                Tax_Withheld = new_payroll.Tax_Withheld,
                Payroll_Adjustment = new_payroll.Payroll_Adjustment,
                Adjustment_Reason = new_payroll.Adjustment_Reason,
                Pay_Period_Start_Date = new_payroll.Pay_Period_Start_Date,
                Pay_Period_End_Date = new_payroll.Pay_Period_End_Date,
                Direct_Deposit_Number = new_payroll.Direct_Deposit_Number,
                Check_Number = new_payroll.Check_Number
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Payroll", new { id = id });
        }

        // GET: Payroll/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? employeeid, int? id, Payroll payroll)
        {
            if (employeeid == null)
            {
                return NotFound();
            }

            var pr = await _context.Payrolls
                .Where(p => p.EmployeeId == employeeid && p.Id == id)
                .SingleOrDefaultAsync();

            ViewData["employeeid"] = employeeid;
            ViewData["payrollid"] = id;

            return View(pr);
        }

        // POST: Payroll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? employeeid, int id, Payroll payroll)
        {
            if (id != payroll.Id)
            {
                return NotFound();
            }

            var pr = await _context.Payrolls
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeid && p.Id == id);

            _logger.LogInformation("Edit payroll - EmployeeId: {employeeid}, PayrollId: {payrollid}, Id: {Id}",
                employeeid, id, payroll.Id);

            if (pr != null)
            {
                pr.EmployeeId = (int)employeeid;
                pr.Processed_Datetime = payroll?.Processed_Datetime;
                pr.Regular_Pay = payroll.Regular_Pay;
                pr.Overtime_Pay = payroll.Overtime_Pay;
                pr.Vacation_Pay = payroll.Vacation_Pay;
                pr.Tax_Withheld = payroll.Tax_Withheld;
                pr.Payroll_Adjustment = payroll.Payroll_Adjustment;
                pr.Adjustment_Reason = payroll.Adjustment_Reason;
                pr.Pay_Period_Start_Date = payroll.Pay_Period_Start_Date;
                pr.Pay_Period_End_Date = payroll.Pay_Period_End_Date;
                pr.Direct_Deposit_Number = payroll.Direct_Deposit_Number;
                pr.Check_Number = payroll.Check_Number;
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Payroll", new { id = employeeid });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(viewModel);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.Id))
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
            return RedirectToAction("Index", "Payroll", new { id = employeeid });
        }

        // GET: Payroll/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? employeeid, int? id, Payroll payroll)
        {
        
            if (employeeid == null)
            {
                return NotFound();
            }

            var pr = await _context.Payrolls
                .Where(p => p.EmployeeId == employeeid && p.Id == id)
                .SingleOrDefaultAsync();

            ViewData["employeeid"] = employeeid;
            ViewData["payrollid"] = id;

            return View(pr);
        }

        // POST: Payroll/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? employeeid, int id, Payroll payroll)
        {
            if (id != payroll.Id)
            {
                return NotFound();
            }

            var pr = await _context.Payrolls
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeid && p.Id == id);

            if (pr != null)
            {
                _context.Payrolls.Remove(pr);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Payroll", new { id = employeeid });
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(viewModel);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.Id))
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
            return RedirectToAction("Index", "Payroll", new { id = employeeid });
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.Id == id);
        }
    }
}