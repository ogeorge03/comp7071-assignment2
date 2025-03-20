using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class Customer_ServiceController : Controller
    {
        private readonly Context _context;

        public Customer_ServiceController(Context context)
        {
            _context = context;
        }

        // GET: Customer_Service
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer_Services.ToListAsync());
        }

        // GET: Customer_Service/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service = await _context.Customer_Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer_Service == null)
            {
                return NotFound();
            }

            return View(customer_Service);
        }

        // GET: Customer_Service/Create
        public IActionResult Create()
        {
            ViewBag.CertificationId = new SelectList(_context.Certifications, "Id", "Certification_Description");

            return View();
        }

        // POST: Customer_Service/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Hourly_Rate")] Customer_Service customer_Service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer_Service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CertificationId = new SelectList(_context.Certifications, "Id", "Certification_Description", customer_Service.Certification_Required);

            return View(customer_Service);
        }

        // GET: Customer_Service/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service = await _context.Customer_Services.FindAsync(id);
            if (customer_Service == null)
            {
                return NotFound();
            }
            return View(customer_Service);
        }

        // POST: Customer_Service/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Hourly_Rate")] Customer_Service customer_Service)
        {
            if (id != customer_Service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer_Service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Customer_ServiceExists(customer_Service.Id))
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
            return View(customer_Service);
        }

        // GET: Customer_Service/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service = await _context.Customer_Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer_Service == null)
            {
                return NotFound();
            }

            return View(customer_Service);
        }

        // POST: Customer_Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer_Service = await _context.Customer_Services.FindAsync(id);
            if (customer_Service != null)
            {
                _context.Customer_Services.Remove(customer_Service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Customer_ServiceExists(int id)
        {
            return _context.Customer_Services.Any(e => e.Id == id);
        }
    }
}
