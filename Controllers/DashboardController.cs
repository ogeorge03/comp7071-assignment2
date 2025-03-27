using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Assignment2.Models;
using Assignment2;

public class EmployeesController : Controller
{
    private readonly Context _context;

    public EmployeesController(Context context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        // Mocked employee ID: replace with real session or claim
        int employeeId = 1; // Replace with dynamic logic

        var assignedServiceIds = await _context.Set<Employee_Service_Assignment>()
            .Where(a => a.EmployeeId == employeeId)
            .Select(a => a.Customer_Service_ScheduledId)
            .ToListAsync();

        var services = await _context.Set<Customer_Service_Scheduled>()
            .Include(s => s.Customer)
            .Include(s => s.Customer_Service)
            .Include(s => s.Facility)
            .Where(s => assignedServiceIds.Contains(s.Id))
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            AssignedScheduledServices = services
        };

        return View("Dashboard/Index", viewModel);
    }
}
