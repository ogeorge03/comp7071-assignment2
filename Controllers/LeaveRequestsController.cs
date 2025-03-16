using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Assignment2.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private readonly Context _context;

        public LeaveRequestsController(Context context)
        {
            _context = context;
        }

        // GET: LeaveRequests
        public async Task<IActionResult> Index()
        {
            var pendingSickLeaves = await _context.Employee_Sick_Leaves.Where(l => l.Status == "Pending").ToListAsync();
            var pendingVacations = await _context.Employee_Vacations.Where(l => l.Status == "Pending").ToListAsync();
            ViewData["sickLeaves"] = pendingSickLeaves;
            ViewData["vacations"] = pendingVacations;
            return View();
        }

        // POST: LeaveRequests/ApproveLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id, string type)
        {
            if (type == "sick")
            {
                var leave = await _context.Employee_Sick_Leaves.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
                _context.Update(leave);
            }
            else if (type == "vacation")
            {
                var leave = await _context.Employee_Vacations.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
                _context.Update(leave);
            }
            else
            {
                return BadRequest("Invalid leave type.");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveRequests/DeclineLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineLeave(int id, string type)
        {
            if (type == "sick")
            {
                var leave = await _context.Employee_Sick_Leaves.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Declined";
                _context.Update(leave);
            }
            else if (type == "vacation")
            {
                var leave = await _context.Employee_Vacations.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Declined";
                _context.Update(leave);
            }
            else
            {
                return BadRequest("Invalid leave type.");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
