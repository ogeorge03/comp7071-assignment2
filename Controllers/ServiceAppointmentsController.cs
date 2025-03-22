using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assignment2.Controllers
{
    public class ServiceAppointmentsController : Controller
    {
        private readonly Context _context;

        public ServiceAppointmentsController(Context context)
        {
            _context = context;
        }

        // GET: ServiceAppointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.ServiceAppointments
                .Include(sa => sa.Customer)
                .Include(sa => sa.Service)
                .ToListAsync();
            return View(appointments);
        }

        // GET: ServiceAppointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.ServiceAppointments
                .Include(sa => sa.Customer)
                .Include(sa => sa.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: ServiceAppointments/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "First_Name");
            ViewData["ServiceId"] = new SelectList(_context.Customer_Services, "Id", "Description");
            return View();
        }

        // POST: ServiceAppointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,ServiceId,AppointmentDate,StartTime,EndTime,Notes")] ServiceAppointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "First_Name", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Customer_Services, "Id", "Description", appointment.ServiceId);
            return View(appointment);
        }


        // GET: ServiceAppointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.ServiceAppointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "First_Name", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Customer_Services, "Id", "Description", appointment.ServiceId);
            return View(appointment);
        }

        // POST: ServiceAppointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,ServiceId,AppointmentDate,StartTime,EndTime,Notes")] ServiceAppointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceAppointmentExists(appointment.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "First_Name", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Customer_Services, "Id", "Description", appointment.ServiceId);
            return View(appointment);
        }

        // GET: ServiceAppointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.ServiceAppointments
                .Include(sa => sa.Customer)
                .Include(sa => sa.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: ServiceAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.ServiceAppointments.FindAsync(id);
            _context.ServiceAppointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceAppointmentExists(int id)
        {
            return _context.ServiceAppointments.Any(e => e.Id == id);
        }
    }
}
