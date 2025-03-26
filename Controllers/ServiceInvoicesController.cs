using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
using System.Threading.Tasks;
using System.Linq;
using Assignment2.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Assignment2.ViewModels;

namespace Assignment2.Controllers
{
    public class ServiceInvoicesController : Controller
    {
        private readonly Context _context;

        private readonly EmailService _emailService;

        public ServiceInvoicesController(Context context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: ServiceInvoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.ServiceInvoices
                .Include(si => si.ServiceAppointment)
                .ThenInclude(sa => sa.Customer)
                .ToListAsync();
            return View(invoices);
        }

        // GET: ServiceInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.ServiceInvoices
                .Include(si => si.ServiceAppointment)
                .ThenInclude(sa => sa.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: ServiceInvoices/Create
        public IActionResult Create()
        {
            ViewData["ServiceAppointmentId"] = new SelectList(_context.ServiceAppointments, "Id", "Id");
            return View();
        }

        // POST: ServiceInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceAppointmentId,Amount,InvoiceDate,PaymentDueDate,IsPaid")] ServiceInvoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceAppointmentId"] = new SelectList(_context.ServiceAppointments, "Id", "Id", invoice.ServiceAppointmentId);
            return View(invoice);
        }

        // GET: ServiceInvoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.ServiceInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["ServiceAppointmentId"] = new SelectList(_context.ServiceAppointments, "Id", "Id", invoice.ServiceAppointmentId);
            return View(invoice);
        }

        // POST: ServiceInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceAppointmentId,Amount,InvoiceDate,PaymentDueDate,IsPaid")] ServiceInvoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceInvoiceExists(invoice.Id))
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
            ViewData["ServiceAppointmentId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.ServiceAppointments, "Id", "Id", invoice.ServiceAppointmentId);
            return View(invoice);
        }

        // GET: ServiceInvoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.ServiceInvoices
                .Include(si => si.ServiceAppointment)
                .ThenInclude(sa => sa.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: ServiceInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.ServiceInvoices.FindAsync(id);
            _context.ServiceInvoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceInvoiceExists(int id)
        {
            return _context.ServiceInvoices.Any(e => e.Id == id);
        }

        // GET: ServiceInvoices/Generate/5
        public async Task<IActionResult> Generate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.ServiceAppointments
                .Include(sa => sa.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            var invoice = new ServiceInvoice
            {
                ServiceAppointmentId = appointment.Id,
                Amount = CalculateAmount(appointment),
                InvoiceDate = DateTime.Now,
                PaymentDueDate = DateTime.Now.AddDays(30),
                IsPaid = false
            };

            _context.ServiceInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Send email to client
            await SendInvoiceEmail(invoice);

            return RedirectToAction(nameof(Index));
        }

        private decimal CalculateAmount(ServiceAppointment appointment)
        {
            decimal hourlyRate = 50.00m;

            double durationInHours = (appointment.EndTime - appointment.StartTime).TotalHours;

            decimal amount = (decimal)durationInHours * hourlyRate;

            return amount;
        }

        private async Task SendInvoiceEmail(ServiceInvoice invoice)
        {
            var customerEmail = invoice.ServiceAppointment.Customer.Email;
            var subject = "Your Service Invoice";
            var body = $"<p>Dear {invoice.ServiceAppointment.Customer.First_Name},</p>" +
                       $"<p>Thank you for using our services. Please find your invoice details below:</p>" +
                       $"<p>Invoice Amount: {invoice.Amount}</p>" +
                       $"<p>Invoice Date: {invoice.InvoiceDate.ToString("yyyy-MM-dd")}</p>" +
                       $"<p>Payment Due Date: {invoice.PaymentDueDate?.ToString("yyyy-MM-dd")}</p>" +
                       $"<p>Best regards,</p>" +
            $"<p>Your Company</p>";

            await _emailService.SendEmailAsync(customerEmail, subject, body);

        }

        // GET: ServiceInvoices/RevenueTrends
        public async Task<IActionResult> RevenueTrends()
        {
            var revenueData = await _context.ServiceInvoices
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new RevenueTrendViewModel
                {
                    Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(i => i.Amount)
                })
                .OrderBy(rt => rt.Period)
                .ToListAsync();

            return View(revenueData);
        }
    }
}
