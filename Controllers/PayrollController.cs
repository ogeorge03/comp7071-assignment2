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
            
            //return View(payroll);
        }

        // GET: Payroll/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeId == id);

            return View(payroll);
        }

        // POST: Payroll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Employee,Processed_Datetime,Regular_Pay,Overtime_Pay,Vacation_Pay,Tax_Withheld,Payroll_Adjustment,Adjustment_Reason,Pay_Period_Start_Date,Pay_Period_End_Date,Direct_Deposit_Number,Check_Number")] Payroll payroll)
        {
            if (id != payroll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
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
            return View(payroll);
        }

        // GET: Payroll/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Payroll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.Id == id);
        }
    }
}