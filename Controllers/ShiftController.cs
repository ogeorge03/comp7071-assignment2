using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers
{
    public class ShiftController : Controller
    {
        private readonly Context _context;
        public ShiftController(Context context)
        {
            _context = context;
        }

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shift_Schedules.Include(s => s.Employee).ToListAsync();
            return View(shifts);
        }

        // GET: Assign Shift
        public IActionResult Assign()
        {
            return View();
        }

        // POST: Assign Shift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign([Bind("EmployeeId, SupervisorId, Start_Datetime, Hours_Scheduled, Comments")] Shift_Schedule shift)
        {
            if (ModelState.IsValid)
            {
                // Prevent overlapping shifts
                bool isConflict = _context.Shift_Schedules.Any(s => s.EmployeeId == shift.EmployeeId &&
                                                                    s.Start_Datetime < shift.Start_Datetime.AddHours(shift.Hours_Scheduled) &&
                                                                    shift.Start_Datetime < s.Start_Datetime.AddHours(s.Hours_Scheduled));

                if (isConflict)
                {
                    ModelState.AddModelError("", "Shift conflicts with an existing assignment.");
                    return View(shift);
                }

                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        // GET: Edit Shift
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var shift = await _context.Shift_Schedules.FindAsync(id);
            if (shift == null) return NotFound();

            return View(shift);
        }

        // POST: Edit Shift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId, SupervisorId, Start_Datetime, Hours_Scheduled, Comments")] Shift_Schedule shift)
        {
            if (id != shift.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        // GET: Delete Shift
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var shift = await _context.Shift_Schedules.FindAsync(id);
            if (shift == null) return NotFound();

            return View(shift);
        }

        // POST: Delete Shift
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shift_Schedules.FindAsync(id);
            if (shift != null)
            {
                _context.Shift_Schedules.Remove(shift);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AvailableEmployeesForShift(DateTime shiftStart)
        {
            var shift = _context.Shift_Schedules.FirstOrDefault(s => s.Start_Datetime == shiftStart);
            if (shift == null)
                return NotFound();

            var assignedIds = _context.Shift_Schedules
                .Where(s => s.Start_Datetime == shiftStart)
                .Select(s => s.EmployeeId)
                .ToList();

            var availableEmployees = _context.Employees
                .Where(e =>
                    !_context.Employee_Sick_Leaves.Any(l => l.EmployeeId == e.Id &&
                        l.Sick_Day.Date == shiftStart.Date) &&
                    !_context.Employee_Vacations.Any(v => v.EmployeeId == e.Id &&
                        shiftStart.Date >= v.Vacation_Start_Date.Date && shiftStart.Date <= v.Vacation_End_Date.Date) &&
                    !assignedIds.Contains(e.Id)
                )
                .ToList();

            var sorted = availableEmployees
                .OrderByDescending(e => (DateTime.Now - e.Employment_Start_Date).TotalDays)
                .ToList();

            return View(sorted);
        }





    }
}
