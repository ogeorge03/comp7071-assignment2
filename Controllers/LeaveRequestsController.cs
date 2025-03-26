using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using Assignment2.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : Controller
    {
        private readonly Context _context;
        private readonly EmailService _emailService;

        public LeaveRequestsController(Context context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("sick-leave")]
        public async Task<IActionResult> RequestSickLeave([FromBody] Employee_Sick_Leave leaveRequest)
        {
            if (leaveRequest.Sick_Day == default)
            {
                return BadRequest(new { message = "Sick day is required." });
            }

            leaveRequest.Status = "Pending";
            _context.Employee_Sick_Leaves.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sick leave request submitted successfully." });
        }

        [HttpPost("vacation")]
        public async Task<IActionResult> RequestVacation([FromBody] Employee_Vacation leaveRequest)
        {
            if (leaveRequest.Vacation_Start_Date == default || leaveRequest.Vacation_End_Date == default)
            {
                return BadRequest(new { message = "Vacation start and end dates are required." });
            }

            leaveRequest.Status = "Pending";
            _context.Employee_Vacations.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vacation request submitted successfully." });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var pendingSickLeaves = await _context.Employee_Sick_Leaves.Where(l => l.Status == "Pending").ToListAsync();
            var pendingVacations = await _context.Employee_Vacations.Where(l => l.Status == "Pending").ToListAsync();
            return Ok(new { sickLeaves = pendingSickLeaves, vacations = pendingVacations });
        }

        // Web-based View for Admin
        public async Task<IActionResult> Index()
        {
            var pendingSickLeaves = await _context.Employee_Sick_Leaves.Where(l => l.Status == "Pending").ToListAsync();
            var pendingVacations = await _context.Employee_Vacations.Where(l => l.Status == "Pending").ToListAsync();
            ViewData["sickLeaves"] = pendingSickLeaves;
            ViewData["vacations"] = pendingVacations;
            return View();
        }

        // POST: ApproveLeave (Web Form)
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

        // POST: DeclineLeave (Web Form)
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
