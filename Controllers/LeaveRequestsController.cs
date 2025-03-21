using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Threading.Tasks;
using System.Linq;
using Assignment2.Services;

namespace Assignment2.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private readonly Context _context;

        public LeaveRequestsController(Context context)
        {
            _context = context;
        }

        private readonly EmailService _emailService;

        public LeaveRequestsController(Context context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
            string leaveType = type == "sick" ? "Sick Leave" : type == "vacation" ? "Vacation Leave" : null;
            if (leaveType == null) return BadRequest("Invalid leave type.");

            int? employeeId = null;

            if (type == "sick")
            {
                var leave = await _context.Employee_Sick_Leaves.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
                _context.Update(leave);
                employeeId = leave.EmployeeId;
            }
            else if (type == "vacation")
            {
                var leave = await _context.Employee_Vacations.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
                _context.Update(leave);
                employeeId = leave.EmployeeId;
            }

            // Fetch employee's email
            if (employeeId.HasValue)
            {
                var employee = await _context.Employees
                .Include(e => e.Contact_Information)
                .FirstOrDefaultAsync(e => e.Id == employeeId.Value);

                var email = employee?.Contact_Information?
                    .FirstOrDefault(c => c.Contact_Type == Contact_Information.CONTACT_TYPE.EMAIL)
                    ?.Contact_Info;

                if (!string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendEmailAsync(
                        email,
                        $"{leaveType} Approved",
                        $"Hello,\n\nYour {leaveType} request (ID: {id}) has been approved.\n\nThank you."
                    );
                }
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
