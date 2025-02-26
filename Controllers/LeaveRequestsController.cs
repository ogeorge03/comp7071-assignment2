using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Assignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly Context _context;

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


        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var pendingSickLeaves = await _context.Employee_Sick_Leaves.Where(l => l.Status == "Pending").ToListAsync();
            var pendingVacations = await _context.Employee_Vacations.Where(l => l.Status == "Pending").ToListAsync();
            return Ok(new { sickLeaves = pendingSickLeaves, vacations = pendingVacations });
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(int id, [FromQuery] string type)
        {
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
    }
}
