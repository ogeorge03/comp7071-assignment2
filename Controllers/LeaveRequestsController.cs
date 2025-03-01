using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly Context _context;
        private readonly List<int> _authorizedEmployeeIds = new List<int> { 1, 2, 3 }; // Example authorized employee IDs

        public LeaveRequestsController(Context context)
        {
            _context = context;
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

        [HttpGet("pending-requests")]
        public async Task<IActionResult> PendingRequests()
        {
            var pendingSickLeaves = await _context.Employee_Sick_Leaves.Where(l => l.Status == "Pending").ToListAsync();
            var pendingVacations = await _context.Employee_Vacations.Where(l => l.Status == "Pending").ToListAsync();
            return Ok(new { sickLeaves = pendingSickLeaves, vacations = pendingVacations });
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(int id, [FromQuery] string type, [FromQuery] int approverId)
        {
            if (!_authorizedEmployeeIds.Contains(approverId))
            {
                return Unauthorized(new { message = "You are not authorized to approve leave requests." });
            }

            if (type == "sick")
            {
                var leave = await _context.Employee_Sick_Leaves.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
            }
            else if (type == "vacation")
            {
                var leave = await _context.Employee_Vacations.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Approved";
            }
            else
            {
                return BadRequest("Invalid leave type.");
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Leave request approved." });
        }

        [HttpPut("decline/{id}")]
        public async Task<IActionResult> DeclineLeave(int id, [FromQuery] string type, [FromQuery] int approverId)
        {
            if (!_authorizedEmployeeIds.Contains(approverId))
            {
                return Unauthorized(new { message = "You are not authorized to decline leave requests." });
            }

            if (type == "sick")
            {
                var leave = await _context.Employee_Sick_Leaves.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Declined";
            }
            else if (type == "vacation")
            {
                var leave = await _context.Employee_Vacations.FindAsync(id);
                if (leave == null) return NotFound();
                leave.Status = "Declined";
            }
            else
            {
                return BadRequest("Invalid leave type.");
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Leave request declined." });
        }
        // POST: Employees/ApproveVacation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVacation(int id)
        {
            var vacation = await _context.Employee_Vacations.FindAsync(id);
            if (vacation == null)
            {
                return NotFound();
            }

            // Logic to approve the vacation
            vacation.Is_Paid_Vacation = true; // Example logic, adjust as needed

            _context.Update(vacation);
            await _context.SaveChangesAsync();
            return RedirectToAction("WorkHistory", new { id = vacation.EmployeeId });
        }

        // POST: Employees/DeclineVacation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineVacation(int id)
        {
            var vacation = await _context.Employee_Vacations.FindAsync(id);
            if (vacation == null)
            {
                return NotFound();
            }

            // Logic to decline the vacation
            _context.Employee_Vacations.Remove(vacation);

            await _context.SaveChangesAsync();
            return RedirectToAction("WorkHistory", new { id = vacation.EmployeeId });
        }

        // POST: Employees/ApproveSickLeave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSickLeave(int id)
        {
            var sickLeave = await _context.Employee_Sick_Leaves.FindAsync(id);
            if (sickLeave == null)
            {
                return NotFound();
            }

            // Logic to approve the sick leave
            sickLeave.Approve();

            _context.Update(sickLeave);
            await _context.SaveChangesAsync();
            return RedirectToAction("WorkHistory", new { id = sickLeave.EmployeeId });
        }

        // POST: Employees/DeclineSickLeave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineSickLeave(int id)
        {
            var sickLeave = await _context.Employee_Sick_Leaves.FindAsync(id);
            if (sickLeave == null)
            {
                return NotFound();
            }

            // Logic to decline the sick leave
            sickLeave.Decline();

            _context.Update(sickLeave);
            await _context.SaveChangesAsync();
            return RedirectToAction("WorkHistory", new { id = sickLeave.EmployeeId });
        }
    }
}
